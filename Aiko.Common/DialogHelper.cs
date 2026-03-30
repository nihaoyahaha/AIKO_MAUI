using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.Common
{
    public static class DialogHelper
    {
        private const string AppTitle = "HISYS 配筋検査システム";

        // 转换原有的 MessageDialog (单按钮通知)
        public static async void MessageDialog(string errMsg)
        {
            await Shell.Current.DisplayAlertAsync(AppTitle, errMsg, "OK");
        }

        public static async void MessageDialogClose(string errMsg)
        {
            await Shell.Current.DisplayAlertAsync(AppTitle, errMsg, "Close");
        }

        public static async void MessageDialogOk(string errMsg)
        {
            await Shell.Current.DisplayAlertAsync(AppTitle, errMsg, "OK");
        }

        /// <summary>
        /// 1个按钮：はい
        /// </summary>
        public static async Task<NCDialogResult> MessageDialogButton1(string errMsg)
        {
            await Shell.Current.DisplayAlertAsync(AppTitle, errMsg, "はい");
            return NCDialogResult.Yes;
        }

        /// <summary>
        /// 2个按钮：はい / いいえ
        /// </summary>
        public static async Task<NCDialogResult> MessageDialogButton2(string errMsg)
        {
            // DisplayAlert 返回 true 代表左边按钮，false 代表右边按钮
            bool result = await Shell.Current.DisplayAlertAsync(AppTitle, errMsg, "はい", "いいえ");
            return result ? NCDialogResult.Yes : NCDialogResult.No;
        }

        /// <summary>
        /// 3个按钮：はい / いいえ / キャンセル
        /// MAUI 的 DisplayAlert 不支持 3 个按钮，必须改用 DisplayActionSheet
        /// </summary>
        public static async Task<NCDialogResult> MessageDialogButton3(string errMsg)
        {
            // 参数说明：标题, 销毁按钮文本(null), 取消按钮文本, 其他按钮
            string result = await Shell.Current.DisplayActionSheetAsync(AppTitle, "キャンセル", null, "はい", "いいえ");

            return result switch
            {
                "はい" => NCDialogResult.Yes,
                "いいえ" => NCDialogResult.No,
                _ => NCDialogResult.Cancel // 包含点击“キャンセル”或点击弹窗外部
            };
        }
    }

    public enum NCDialogResult
    {
        Yes,    // はい
        No,     // いいえ
        Cancel  // キャンセル
    }
}
