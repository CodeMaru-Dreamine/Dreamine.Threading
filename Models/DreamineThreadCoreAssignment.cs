namespace Dreamine.Threading.Models;

/// <summary>
/// \if KO
/// <para>Dreamine 작업자 스레드의 CPU 코어 할당 결과를 나타냅니다.</para>
/// \endif
/// \if EN
/// <para>Represents the CPU core assignment result for a Dreamine worker thread.</para>
/// \endif
/// </summary>
public sealed class DreamineThreadCoreAssignment
{
    /// <summary>
    /// \if KO
    /// <para>할당된 CPU 코어 인덱스를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the assigned CPU core index.</para>
    /// \endif
    /// </summary>
    public int? CoreIndex { get; }

    /// <summary>
    /// \if KO
    /// <para>CPU 선호도를 적용해야 하는지 여부를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets whether CPU affinity should be applied.</para>
    /// \endif
    /// </summary>
    public bool UseAffinity { get; }

    /// <summary>
    /// \if KO
    /// <para>이 할당이 오버플로 폴링 작업을 나타내는지 여부를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets whether this assignment represents an overflow polling job.</para>
    /// \endif
    /// </summary>
    public bool IsOverflowPolling { get; }

    /// <summary>
    /// \if KO
    /// <para>이 할당이 전용 작업자 스레드를 가지는지 여부를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets whether this assignment has a dedicated worker thread.</para>
    /// \endif
    /// </summary>
    public bool IsDedicatedWorker => !IsOverflowPolling;

    /// <summary>
    /// \if KO
    /// <para><see cref="T:Dreamine.Threading.Models.DreamineThreadCoreAssignment" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.Threading.Models.DreamineThreadCoreAssignment" />.</para>
    /// \endif
    /// </summary>
    /// <param name="coreIndex">
    /// \if KO
    /// <para>할당된 CPU 코어 인덱스이며 코어가 없으면 <see langword="null"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The assigned CPU core index, or <see langword="null"/> when no core is assigned.</para>
    /// \endif
    /// </param>
    /// <param name="useAffinity">
    /// \if KO
    /// <para>CPU 선호도를 적용할지 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether CPU affinity should be applied.</para>
    /// \endif
    /// </param>
    /// <param name="isOverflowPolling">
    /// \if KO
    /// <para>오버플로 폴링 할당인지 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether the assignment is for overflow polling.</para>
    /// \endif
    /// </param>
    public DreamineThreadCoreAssignment(int? coreIndex, bool useAffinity, bool isOverflowPolling)
    {
        CoreIndex = coreIndex;
        UseAffinity = useAffinity;
        IsOverflowPolling = isOverflowPolling;
    }

    /// <summary>
    /// \if KO
    /// <para>CPU 선호도나 전용 코어가 없는 할당을 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates an assignment without CPU affinity or a dedicated core.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>생성된 비선호도 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The created no-affinity assignment.</para>
    /// \endif
    /// </returns>
    public static DreamineThreadCoreAssignment None()
    {
        return new DreamineThreadCoreAssignment(null, false, false);
    }

    /// <summary>
    /// \if KO
    /// <para>전용 CPU 코어 할당을 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a dedicated CPU core assignment.</para>
    /// \endif
    /// </summary>
    /// <param name="coreIndex">
    /// \if KO
    /// <para>할당할 CPU 코어 인덱스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The CPU core index to assign.</para>
    /// \endif
    /// </param>
    /// <param name="useAffinity">
    /// \if KO
    /// <para>해당 코어에 선호도를 적용할지 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether affinity should be applied to that core.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>생성된 전용 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The created dedicated assignment.</para>
    /// \endif
    /// </returns>
    public static DreamineThreadCoreAssignment Dedicated(int coreIndex, bool useAffinity)
    {
        return new DreamineThreadCoreAssignment(coreIndex, useAffinity, false);
    }

    /// <summary>
    /// \if KO
    /// <para>전용 작업자 없이 실행할 오버플로 폴링 할당을 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates an overflow polling assignment without a dedicated worker.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>생성된 오버플로 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The created overflow assignment.</para>
    /// \endif
    /// </returns>
    public static DreamineThreadCoreAssignment Overflow()
    {
        return new DreamineThreadCoreAssignment(null, false, true);
    }
}
