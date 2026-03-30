using System;
namespace Aiko.Common;

public static class ErrorMessage
{

    public static string ERRORPOP(string ERRORID)
    {
		string error = "";
        switch (ERRORID)
        {
            case "CM00001":
                error = "エラーが発生しました、システム管理者ご連絡ください。";
                break;
            case "CM00002":
                error = "登録しました。";
                break;
            case "CM00003":
                error = "オペレーター・パスワードの入力に誤りがあります。もう一度、正しく入力してください";
                break;
            case "CM00004":
                error = "試す回数は３回を超えました、不正アクセスを防ぐために終了させていただきます。";
                break;
            case "CM00006":
                error = "サービスに接続できません。IPアドレスとポートをチェックしてください。";
                break;
            case "CM00101":
                error = "未入力の情報があります、処理できません。";
                break;
            case "CM00102":
                error = "オペレータコードは6桁の英数字を入力してください。";
                break;
            case "CM00104":
                error = "エラーが発生しました、処理失敗しました。";
                break;
            case "CM00105":
                error = "ディスク容量不足のため、処理に失敗しました。";
                break;
            case "CM01000":
                error = "ファイルを削除します、よろしいですか？";
                break;
            case "CM01001":
                error = "未入力の情報があります、処理できません。";
                break;
            case "CM01002":
                error = "会社が変わるため、プロジェクトユーザーをクリアします、\r\nよろしいですか？";
                break;
            case "CM01003":
                error = "プロジェクト基本情報を登録しました。";
                break;
            case "CM01004":
                error = "エラーが発生しました、処理失敗しました。";
                break;
            case "CM01005":
                error = "プロジェクトユーザーを登録しました。";
                break;
            case "CM01006":
                error = "プロジェクトファイルを登録しました。";
                break;
            case "CM01007":
                error = "プロジェクト階を登録しました。";
                break;
            case "CM01008":
                error = "プロジェクト工程を登録しました。";
                break;
            case "CM01009":
                error = "プロジェクト部位を登録しました。";
                break;
            case "CM01010":
                error = "プロジェクトメモを登録しました。";
                break;
            case "CM01012":
                error = "プロジェクト確認項目を登録しました。";
                break;
            case "CM01013":
                error = "プロジェクト工区を登録しました。";
                break;
            case "CM01014":
                error = "断面の変更を保存しますか？";
                break;
            case "CM01015":
                error = "空白は指定できません。";
                break;
            case "CM01016":
                error = "選択中のグループや断面を削除します、よろしいですか？";
                break;
            case "CM01017":
                error = "下位項目があるため、グループが削除されませんでした。";
                break;
            case "CM01018":
                error = "選択中のマップを削除します、よろしいですか？";
                break;
            case "CM01019":
                error = "選択中のマップは下位のマップが存在するので削除できません。";
                break;
            case "CM01020":
                error = "マップの変更を保存しますか？";
                break;
            case "CM01021":
                error = "既に存在するガイドを作成することはできません。";
                break;
            case "CM01022":
                error = "ガイドを登録しました。";
                break;
            case "CM01023":
                error = "制御点が図面の有効範囲を超えるので移動できません。";
                break;
            case "CM01024":
                error = "他の制御点とX座標が異なっていなければなりません。";
                break;
            case "CM01025":
                error = "確認日または指摘日は入力必須です。";
                break;
            case "CM01026":
                error = "図面ファイルは見つかりません。";
                break;
            case "CM01027":
                error = "選択された現場データをこの端末から全て削除します。よろしいですか？";
                break;
            case "CM01028":
                error = "全ての現場データをこの端末から全て削除します。よろしいですか？";
                break;
            case "CM01029":
                error = "カメラが見つかりません、撮影できません。";
                break;
            case "CM01030":
                error = "完了しました。";
                break;
            case "CM01031":
                error = "撮影した写真を廃棄します、よろしいですか？";
                break;
            case "CM01032":
                error = "作業完了の現場のダウンロード、または同期ができません。";
                break;
            case "CM01033":
                error = "最新バージョンは{0}です。最新バージョンにアップデート後、ダウンロードから実行してください。";
                break;
            case "CM01034":
                error = "検査結果がサーバに反映されないことがあるため、再度同期、又はダウンロードしてください。";
                break;
            case "CM01035":
                error = "処理に失敗しました。ネットワーク接続等を確認してください。";
                break;
            case "CM01036":
                error = "別のアプリはカメラを使用しています。他のアプリを閉じてください。";
                break;
            case "CM01037":
                error = "{0}を選択してください。";
                break;
            case "CM01038":
                error = "ダウンロードの{0}がないため、サーバにファイルを確認した上に、再度ダウンロードしてください。";
                break;
            case "CM01039":
                error = "編集内容を保存しますか？";
                break;
            case "CM01099":
                error = "編集したデータを廃棄します、よろしいですか？";
                break;
            case "CM01127":
                error = "IPアドレス制限のため、接続できません。\r\n貴社のシステム担当者へ確認してください。";
                break;
            case "CM01136":
                error = "端末制限のため、接続できません。\r\n貴社のシステム担当者へ確認してください。";
                break;
            case "CM90001":
                error = "リモートサーバーを入力してください。";
                break;
            case "CM90002":
                error = "リモートポートを入力してください。";
                break;
            case "CM90003":
                error = "設定はシステム次回起動から有効になります。";
                break;
            case "CM90004":
                error = "設定は変更できません。";
                break;
            case "CM90005":
                error = "有効なリモートサーバーではありません。";
                break;
            case "CM90006":
                error = "有効なプロキシサーバーではありません。";
                break;
            case "CM90007":
                error = "プロキシポートを指定してください。";
                break;
            case "CM90008":
                error = "会社名を入力してください。";
                break;
            case "CM90009":
                error = "ファイルサーバに誤りがあります。";
                break;
            case "CM99001":
                error = "リモートサーバにアクセスできません。システム管理者にご連絡ください。";
                break;
            case "CM99002":
                error = "最新バージョンではありません。予期しないエラーを起こす恐れがあります。システム管理者にご連絡ください。";
                break;
            case "CM99003":
                error = "サーバーへの接続に失敗しました、ローカルネットワーク接続を確認してください。";
                break;

        }
        return error;
    }
    public static string ERRORPOP(string ERRORID, string CODE)
    {
        string error = "";
        switch (ERRORID)
        {
            case "CM00100":
                error = CODE + "を削除します、よろしいですか？";
                break;
            case "CM00103":
                error = CODE + "を登録しました。";
                break;
            case "CM01011":
                error = CODE + "は登録してない、確認項目は入力できません。";
                break;
        }
        return error;
    }

    public static string ERRORPOP(string ERRORID, string CODE, string ACODE)
    {
        string error = "";
        switch (ERRORID)
        {
            case "CM00005":
                error = "ローカル日付（" + CODE + "）とサーバ日付（" + ACODE + "）が一致しません。";
                break;
        }
        return error;
    }
}

