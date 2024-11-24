using System;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetModule
{
    /// <summary>
    /// 操作基类
    /// </summary>
    public class Operation : CustomYieldInstruction
    {
        /// <summary>
        /// 协程等待标志
        /// </summary>
        public override bool keepWaiting => !IsDone;

        /// <summary>
        /// 操作完成回调
        /// </summary>
        public Action<Operation> completed;

        /// <summary>
        /// 当前状态
        /// </summary>
        public OperationStatus Status { get; protected set; } = OperationStatus.Init;

        /// <summary>
        /// 进度(取值范围:0~1)
        /// </summary>
        public float Progress { get; protected set; }

        /// <summary>
        /// 操作是否完成
        /// </summary>
        public bool IsDone => Status is OperationStatus.Successful or OperationStatus.Failed;

        /// <summary>
        /// 错误原因
        /// </summary>
        public string Error { get; protected set; }

        /// <summary>
        /// 开始操作
        /// </summary>
        internal void Start()
        {
            Status = OperationStatus.Processing;
            OperationHandler.AddOperation(this);
            OnStart();
        }

        internal void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// 取消操作
        /// </summary>
        public void Cancel()
        {
            Finish("取消");
        }

        /// <summary>
        /// 完成操作(操作成功和失败都需由基类调用此接口)
        /// </summary>
        /// <param name="errorCode">操作失败原因</param>
        protected void Finish(string errorCode = null)
        {
            Error = errorCode;
            Status = string.IsNullOrEmpty(errorCode) ? OperationStatus.Successful : OperationStatus.Failed;
            Progress = 1;
        }

        /// <summary>
        /// 完成通知, 基类调用Finish后, Complete就会被Handler调用
        /// </summary>
        internal void Complete()
        {
            if (completed == null)
                return;

            var func = completed;
            completed = null;
            func.Invoke(this);
        }

        /// <summary>
        /// 开始通知, 由子类继承实现
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Update通知, 由子类继承实现
        /// </summary>
        protected virtual void OnUpdate()
        {
        }

        /// <summary>
        /// 提供给await使用
        /// </summary>
        public Task<Operation> Task
        {
            get
            {
                TaskCompletionSource<Operation> tcs = new();
                completed += _ => tcs.SetResult(this);
                return tcs.Task;
            }
        }
    }
}