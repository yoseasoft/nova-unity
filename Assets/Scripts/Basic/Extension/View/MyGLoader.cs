/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

namespace GameEngine
{
    /// <summary>
    /// 扩展FairyGUI默认GLoader，使其可以加载外部图集
    /// </summary>
    public class MyGLoader : FairyGUI.GLoader
    {
        /// <summary>
        /// 等待显示的URL标识, 不能使用空字符串, 因为若代码真正赋空没办法知道, 但_waitLoadUrl还在, 就会误加载回去
        /// </summary>
        const string WaitLoadTag = "wait://";

        /// <summary>
        /// 等待显示时加载的url
        /// </summary>
        string _waitLoadUrl;

        protected override void LoadExternal()
        {
            // 等待加载中, 不正式加载
            if (url.Equals(WaitLoadTag))
                return;

            // 更换url时, 首先清空等待记录
            _waitLoadUrl = null;

            // 每次设置url都重新添加监听, 以免放构造函数中途监听被清除掉(例如富文本图片回到池里面会清掉委托)
            onAddedToStage.Add(OnAddedToStage);
            onRemovedFromStage.Add(OnRemovedFromStage);

            if (!onStage)
            {
                _waitLoadUrl = url;
                url = WaitLoadTag;
                return;
            }

            FairyGUIHelper.LoadExternalIcon(url, OnLoadSuccess, OnLoadFail);
        }

        /// <summary>
        /// 加载成功
        /// </summary>
        void OnLoadSuccess(FairyGUI.NTexture nTexture, string textureUrl)
        {
            // 因为通常是异步的,所以加载完成后需要判断自身是否已销毁,url是否还相同
            if (isDisposed || string.IsNullOrEmpty(url) || url.Equals(WaitLoadTag))
                return;

            string curUrl = url.Split(FairyGUIHelper.SplitSymbol)[0];
            if (!curUrl.Equals(textureUrl))
                return;

            if (!onStage)
            {
                _waitLoadUrl = url;
                url = WaitLoadTag;
                return;
            }

            onExternalLoadSuccess(nTexture);
        }

        /// <summary>
        /// 加载失败
        /// </summary>
        void OnLoadFail(string textureUrl)
        {
            // 因为通常是异步的,所以加载完成后需要判断自身是否已销毁,url是否还相同
            if (isDisposed || string.IsNullOrEmpty(url) || url.Equals(WaitLoadTag))
                return;

            string curUrl = url.Split(FairyGUIHelper.SplitSymbol)[0];
            if (!curUrl.Equals(textureUrl))
                return;

            onExternalLoadFailed();
#if UNITY_EDITOR
            Debugger.Error($"加载图片失败, 资源路径:{textureUrl}");
#endif
        }

        /// <summary>
        /// 重新回到舞台处理(即窗口重新显示时调用)
        /// 贴图重新赋值
        /// </summary>
        void OnAddedToStage(FairyGUI.EventContext _)
        {
            if (string.IsNullOrEmpty(_waitLoadUrl))
                return;

            // 重新回到舞台时, url已被逻辑代码清空
            if (string.IsNullOrEmpty(url))
            {
                _waitLoadUrl = null;
                return;
            }

            url = _waitLoadUrl;
            _waitLoadUrl = null;
        }

        /// <summary>
        /// 移出舞台处理(即窗口隐藏时调用)
        /// 清空外部加载贴图, 以免占用内存
        /// </summary>
        void OnRemovedFromStage(FairyGUI.EventContext _)
        {
            if (string.IsNullOrEmpty(url) || url.StartsWith(FairyGUI.UIPackage.URL_PREFIX))
                return;

            _waitLoadUrl = url;
            url = WaitLoadTag;
        }
    }
}
