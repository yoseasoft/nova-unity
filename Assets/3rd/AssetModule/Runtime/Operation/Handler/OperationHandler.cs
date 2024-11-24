using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// Operation对象管理
    /// </summary>
    internal static class OperationHandler
    {
        /// <summary>
        /// 处理中的Operation对象列表
        /// </summary>
        static readonly List<Operation> ProcessingList = new();

        /// <summary>
        /// 添加Operation对象
        /// </summary>
        internal static void AddOperation(Operation operation)
        {
            ProcessingList.Add(operation);
        }

        /// <summary>
        /// Update所有Operation对象
        /// </summary>
        internal static void UpdateAllOperations()
        {
            // 需要按顺序Update, 所以虽然要Remove也不倒着来遍历
            for (int i = 0; i < ProcessingList.Count; i++)
            {
                if (AssetDispatcher.Instance.IsBusy)
                    return;

                Operation operation = ProcessingList[i];
                operation.Update();

                if (operation.IsDone)
                {
                    ProcessingList.RemoveAt(i);
                    i--;
                    if (operation.Status == OperationStatus.Failed)
                        Logger.Warning($"操作{operation.GetType().Name}未完成, 原因:{operation.Error}");
                    operation.Complete();
                }
            }
        }
    }
}