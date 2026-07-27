using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Allocators;

/// <summary>
/// \if KO
/// <para>코어별 용량 정책을 사용해 Dreamine 작업자 스레드에 CPU 코어를 자동 할당합니다.</para>
/// \endif
/// \if EN
/// <para>Allocates CPU cores to Dreamine worker threads using an automatic per-core capacity policy.</para>
/// \endif
/// </summary>
public sealed class AutoCoreAllocator : IThreadCoreAllocator
{
    /// <summary>
    /// \if KO
    /// <para>sync Root 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the sync root value.</para>
    /// \endif
    /// </summary>
    private readonly object _syncRoot = new();
    /// <summary>
    /// \if KO
    /// <para>logical Core Count 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the logical core count value.</para>
    /// \endif
    /// </summary>
    private readonly int _logicalCoreCount;
    /// <summary>
    /// \if KO
    /// <para>assigned Counts 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the assigned counts value.</para>
    /// \endif
    /// </summary>
    private readonly int[] _assignedCounts;

    /// <summary>
    /// \if KO
    /// <para>현재 환경의 논리 프로세서 수로 <see cref="T:Dreamine.Threading.Allocators.AutoCoreAllocator" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.Threading.Allocators.AutoCoreAllocator" /> using the environment's logical processor count.</para>
    /// \endif
    /// </summary>
    public AutoCoreAllocator()
        : this(Environment.ProcessorCount)
    {
    }

    /// <summary>
    /// \if KO
    /// <para>지정한 논리 코어 수로 <see cref="T:Dreamine.Threading.Allocators.AutoCoreAllocator" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.Threading.Allocators.AutoCoreAllocator" /> with the specified logical-core count.</para>
    /// \endif
    /// </summary>
    /// <param name="logicalCoreCount">
    /// \if KO
    /// <para>사용할 논리 CPU 코어 수이며 0 이하는 1로 보정됩니다.</para>
    /// \endif
    /// \if EN
    /// <para>The logical CPU core count; values at or below zero are normalized to one.</para>
    /// \endif
    /// </param>
    public AutoCoreAllocator(int logicalCoreCount)
    {
        _logicalCoreCount = logicalCoreCount <= 0 ? 1 : logicalCoreCount;
        _assignedCounts = new int[_logicalCoreCount];
    }

    /// <summary>
    /// \if KO
    /// <para>정규화된 스레드 옵션의 코어 모드에 따라 코어 할당을 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a core assignment according to the normalized thread option's core mode.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>코어 모드와 용량 설정을 포함하는 스레드 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread options containing core mode and capacity settings.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>비선호도, 수동, 자동 또는 오버플로 코어 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A no-affinity, manual, automatic, or overflow core assignment.</para>
    /// \endif
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="options"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="options"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// \if KO
    /// <para>수동 코어 인덱스가 논리 코어 범위를 벗어날 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when a manual core index is outside the logical-core range.</para>
    /// \endif
    /// </exception>
    public DreamineThreadCoreAssignment Allocate(DreamineThreadOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var normalized = options.Normalize();

        lock (_syncRoot)
        {
            return normalized.CoreMode switch
            {
                DreamineThreadCoreMode.None => DreamineThreadCoreAssignment.None(),
                DreamineThreadCoreMode.Manual => AllocateManual(normalized),
                DreamineThreadCoreMode.Auto => AllocateAuto(normalized),
                _ => DreamineThreadCoreAssignment.None()
            };
        }
    }

    /// <summary>
    /// \if KO
    /// <para>전용 코어 할당의 사용 카운트를 감소시킵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Decrements the usage count for a dedicated core assignment.</para>
    /// \endif
    /// </summary>
    /// <param name="assignment">
    /// \if KO
    /// <para>해제할 코어 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The core assignment to release.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="assignment"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="assignment"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public void Release(DreamineThreadCoreAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (assignment.CoreIndex is null || assignment.IsOverflowPolling)
        {
            return;
        }

        lock (_syncRoot)
        {
            var coreIndex = assignment.CoreIndex.Value;

            if (coreIndex < 0 || coreIndex >= _assignedCounts.Length)
            {
                return;
            }

            if (_assignedCounts[coreIndex] > 0)
            {
                _assignedCounts[coreIndex]--;
            }
        }
    }

    /// <summary>
    /// \if KO
    /// <para>유효성을 검사한 수동 코어 할당을 만들고 사용 카운트를 증가시킵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a validated manual core assignment and increments its usage count.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>수동 코어 인덱스를 포함하는 정규화된 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The normalized options containing the manual core index.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>수동 전용 할당이며 인덱스가 없으면 비선호도 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A manual dedicated assignment, or a no-affinity assignment when no index is specified.</para>
    /// \endif
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// \if KO
    /// <para>코어 인덱스가 논리 코어 범위를 벗어날 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the core index is outside the logical-core range.</para>
    /// \endif
    /// </exception>
    private DreamineThreadCoreAssignment AllocateManual(DreamineThreadOptions options)
    {
        if (options.CoreIndex is null)
        {
            return DreamineThreadCoreAssignment.None();
        }

        var coreIndex = options.CoreIndex.Value;

        if (coreIndex < 0 || coreIndex >= _logicalCoreCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                $"CPU core index {coreIndex} is out of range. Logical core count is {_logicalCoreCount}.");
        }

        _assignedCounts[coreIndex]++;
        return DreamineThreadCoreAssignment.Dedicated(coreIndex, true);
    }

    /// <summary>
    /// \if KO
    /// <para>사용 카운트가 가장 낮은 코어를 선택하고 용량이 가득 차면 오버플로 할당을 반환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Selects the least-used core and returns an overflow assignment when all core capacity is full.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>코어당 최대 스레드 수를 포함하는 정규화된 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The normalized options containing maximum threads per core.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>자동 전용 또는 오버플로 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>An automatic dedicated or overflow assignment.</para>
    /// \endif
    /// </returns>
    private DreamineThreadCoreAssignment AllocateAuto(DreamineThreadOptions options)
    {
        var maxPerCore = options.AutoThreadsPerCore <= 0 ? 2 : options.AutoThreadsPerCore;

        var selectedCore = 0;
        var selectedCount = _assignedCounts[0];

        for (var i = 1; i < _assignedCounts.Length; i++)
        {
            if (_assignedCounts[i] < selectedCount)
            {
                selectedCore = i;
                selectedCount = _assignedCounts[i];
            }
        }

        if (selectedCount >= maxPerCore)
        {
            return DreamineThreadCoreAssignment.Overflow();
        }

        _assignedCounts[selectedCore]++;
        return DreamineThreadCoreAssignment.Dedicated(selectedCore, true);
    }
}
