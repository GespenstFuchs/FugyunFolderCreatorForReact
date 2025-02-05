namespace FugyunFolderCreatorForReact.Server.Core
{
    /// <summary>
    /// ふぎゅんコア
    /// </summary>
    public class FugyunCore
    {
        #region フィールド

        /// <summary>
        /// 使用不可文字リスト
        /// </summary>
        private readonly List<string> InvalidCharList;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FugyunCore()
        {
            // 使用不可文字リストを生成する。
            InvalidCharList = [];
            Array.ForEach(Path.GetInvalidFileNameChars(), invalidChar => InvalidCharList.Add(invalidChar.ToString()));
        }

        #endregion

        /// <summary>
        /// フォルダ作成処理
        /// </summary>
        /// <param name="pathText">パステキスト</param>
        /// <returns>メッセージ（true：ブランク・false：エラーメッセージ）</returns>
        public string FolderCreateTran(string pathText)
        {
            try
            {
                // 未入力チェック
                if (string.IsNullOrWhiteSpace(pathText))
                {
                    return "フォルダパスが未入力です。";
                }

                // ドライブ文字リストを取得・保持する。
                List<string> driveLetterList = GetDriveLetterList(),
                    createPathList = [];

                string path, errorMsg, tempPath = string.Empty;
                string[] pathAr = pathText.Split(Const.NewLineAr, StringSplitOptions.None);

                // 入力されたパスを基に処理を行う。
                for (int index = 0; index < pathAr.Length; index++)
                {
                    // パスを保持する。
                    path = pathAr[index];

                    // パスの有無を判定する。
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        // ドライブ文字を、大文字に変換し、保持する。
                        path = string.Concat(path[0].ToString().ToUpper(), path[1..]);

                        // チェック処理を行い、結果を判定する。
                        errorMsg = CheckTran(path, driveLetterList, index + 1);
                        if (string.IsNullOrEmpty(errorMsg))
                        {
                            // 正常の場合

                            // 全フォルダ名を基にループさせる。
                            foreach (string folderName in path.Split(Const.YenAr, StringSplitOptions.RemoveEmptyEntries))
                            {
                                // 予約語の有無を判定する。
                                if (Const.InvalidFolderNameList.Any(word => string.Equals(word, folderName.ToUpper())))
                                {
                                    // 存在する場合

                                    // 処理を終了する。
                                    return string.Format("{0}行目：フォルダ名に使用出来ない文字列（予約語）が含まれています。", ConvertNumberWide(index + 1));
                                }
                            }

                            // 作成パスリストに、パスを追加する。
                            createPathList.Add(path);
                        }
                        else
                        {
                            // エラーの場合、処理を終了する。
                            return errorMsg;
                        }
                    }
                }

                int folderNo = 0, rowIndex = 1;

                try
                {
                    // 仮フォルダを作成する。
                    bool createFlg = false;
                    do
                    {
                        // 仮フォルダパスを生成する。
                        tempPath = string.Concat(driveLetterList[0], Const.Yen, folderNo);

                        // フォルダ検索処理を呼び出し、結果の有無を判定する。
                        if (IsFolderExist(tempPath))
                        {
                            // 存在する場合
                            folderNo++;
                        }
                        else
                        {
                            // 存在しない場合

                            // 仮フォルダを生成し、作成フラグを設定する。
                            Directory.CreateDirectory(tempPath);
                            createFlg = true;
                        }
                    } while (!createFlg);

                    // 仮フォルダ下にフォルダを作成する。
                    for (int index = 0; index < pathAr.Length; index++)
                    {
                        path = pathAr[index];

                        // パスの有無を判定する。
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            // パスが存在している場合

                            // 作成するフォルダパスを生成し、フォルダを作成する。
                            Directory.CreateDirectory(string.Concat(tempPath, path[2..]));
                        }

                        rowIndex++;
                    }

                    // 仮フォルダを削除する。
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(
                        tempPath,
                        Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                        Microsoft.VisualBasic.FileIO.RecycleOption.DeletePermanently,
                        Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);

                    // フォルダを作成する。
                    createPathList.ForEach(createPath => Directory.CreateDirectory(createPath));

                    return string.Empty;

                }
                catch (Exception ex)
                {
                    // 仮フォルダのパスの保持の有無を判定する。
                    if (!string.IsNullOrEmpty(tempPath) && IsFolderExist(tempPath))
                    {
                        // 保持されている場合

                        // 仮フォルダを削除する。
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(
                            tempPath,
                            Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                            Microsoft.VisualBasic.FileIO.RecycleOption.DeletePermanently,
                            Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                    }

                    return ex.Message;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// フォルダ削除処理
        /// </summary>
        /// <param name="pathText">パステキスト</param>
        /// <param name="allDeleteFlg">全削除フラグ（true：全削除・false：削除）</param>
        /// <returns>メッセージ（true：ブランク・false：エラーメッセージ）</returns>
        public string FolderDeleteTran(string pathText, bool allDeleteFlg)
        {
            try
            {
                // 未入力チェック
                if (string.IsNullOrWhiteSpace(pathText))
                {
                    return "フォルダパスが未入力です。";
                }

                // ドライブ文字リストを取得・保持する。
                List<string> driveLetterList = GetDriveLetterList(), deletePathList = [];

                string path, errorMsg, tempPath = string.Empty;
                string[] pathAr = pathText.Split(Const.NewLineAr, StringSplitOptions.None);

                // 入力されたパスを基に処理を行う。
                for (int index = 0; index < pathAr.Length; index++)
                {
                    // パスを保持する。
                    path = pathAr[index];

                    // パスの有無を判定する。
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        // ドライブ文字を、大文字に変換し、保持する。
                        path = string.Concat(path[0].ToString().ToUpper(), path[1..]);

                        // チェック処理を行い、結果を判定する。
                        // （本来フォルダ存在チェックのみで良いが、エラー発生時、エラー行位置がわからないため、チェックを行う。）
                        errorMsg = CheckTran(path, driveLetterList, index + 1);
                        if (string.IsNullOrEmpty(errorMsg))
                        {
                            // 正常の場合

                            // フォルダを検索し、結果を判定する。
                            if (IsFolderExist(path))
                            {
                                // 存在する場合
                                // 全削除フラグを判定する。
                                if (allDeleteFlg)
                                {
                                    // 全削除の場合

                                    // パスを、【ドライブ文字:\フォルダ】の形式にする。
                                    tempPath = string.Join(Const.Yen, path.Split(Const.YenAr, StringSplitOptions.None).Take(2));
                                }
                                else
                                {
                                    // 削除の場合
                                    tempPath = path;
                                }

                                // 削除パスリストに、パスを追加する。
                                deletePathList.Add(tempPath);
                            }
                        }
                        else
                        {
                            // エラーの場合、処理を終了する。
                            return errorMsg;
                        }
                    }
                }

                // 削除パスリストの有無を判定する。
                if (deletePathList.Count != 0)
                {
                    // 存在する場合、削除パスリストを一意にし、ループさせる。。
                    foreach (string deletePath in deletePathList.Distinct())
                    {
                        // フォルダ検索処理を行い、存在する場合、フォルダを削除する。
                        if (IsFolderExist(deletePath))
                        {
                            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(
                                deletePath,
                                Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                                Microsoft.VisualBasic.FileIO.RecycleOption.DeletePermanently,
                                Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                        }
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// チェック処理
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="driveLetterList">ドライブ文字リスト</param>
        /// <param name="rowIndex">行数</param>
        /// <returns>メッセージ（true：ブランク・false：エラーメッセージ）</returns>
        private string CheckTran(string path, List<string> driveLetterList, int rowIndex)
        {
            // 文字数チェック
            if (240 < path.Length)
            {
                // 文字数が２４０文字以上の場合、エラーとする。
                return string.Format(@"{0}行目：パスの入力可能文字数は、２４０文字までです。
入力文字数：{1}文字", ConvertNumberWide(rowIndex), ConvertNumberWide(path.Length));
            }

            // 無名フォルダチェック
            if (!Equals(-1, path.IndexOf(Const.Double_Yen)))
            {
                // 無名フォルダが存在する場合、エラーとする。
                return string.Format("{0}行目：フォルダ名が指定されていない箇所があります。", ConvertNumberWide(rowIndex));
            }

            // パスを配列化する。
            // （末端要素がブランクの場合、削除する。（スペースの場合は削除しない。））
            string[] folderNameAr = path.Split(Const.YenAr, StringSplitOptions.None);
            if (string.IsNullOrEmpty(folderNameAr.Last()))
            {
                folderNameAr = folderNameAr.Take(folderNameAr.Length - 1).ToArray();
            }

            // 要素数チェック
            if (2 > folderNameAr.Length)
            {
                // 要素数が２個以下の場合、エラーとする。
                return string.Format("{0}行目：フォルダ名が入力されていません。", ConvertNumberWide(rowIndex));
            }

            // パス使用文字チェック
            // ドライブ文字以外で、ループさせる。
            foreach (string folderName in folderNameAr.Skip(1))
            {
                foreach (string invalidChar in InvalidCharList)
                {
                    if (folderName.Contains(invalidChar))
                    {
                        // パスに使用出来ない文字が使用されている場合、エラーとする。
                        return string.Format("{0}行目：フォルダ名に使用出来ない文字が含まれています。", ConvertNumberWide(rowIndex));
                    }
                }
            }

            // スペース・ドットチェック
            foreach (string folderName in folderNameAr)
            {
                if (string.IsNullOrWhiteSpace(folderName))
                {
                    // スペースのみのフォルダが存在する場合、エラーとする。
                    return string.Format("{0}行目：スペース（全角・半角問わず）のみのフォルダは、処理出来ません。", ConvertNumberWide(rowIndex));
                }
                else if (string.Equals(Const.Dot, folderName))
                {
                    // ドットのみのフォルダが存在する場合、エラーとする。
                    return string.Format("{0}行目：【.】のみのフォルダは、処理出来ません。", ConvertNumberWide(rowIndex));
                }
                else if (folderName.EndsWith(Const.Dot))
                {
                    // フォルダ名の末尾が【.】の場合、エラーとする。
                    return string.Format("{0}行目：フォルダ名の末尾が【.】の場合、処理出来ません。", ConvertNumberWide(rowIndex));
                }
            }

            // ドライブ文字を保持する。
            string driveLetter = path[..2];

            // ドライブチェック
            if (!driveLetterList.Contains(driveLetter))
            {
                // 存在していないドライブ文字が指定されている場合、エラーとする。
                return string.Format("{0}行目：存在しないドライブ文字（C:やD:）が入力されています。", ConvertNumberWide(rowIndex));
            }

            return string.Empty;
        }

        /// <summary>
        /// フォルダ検索処理
        /// </summary>
        /// <param name="searchPath">検索パス</param>
        /// <returns>検索結果（true：フォルダ有り・false：フォルダ無し）</returns>
        private static bool IsFolderExist(string searchPath)
        {
            string targetPath;

            string[] pathAr = searchPath.Split(Const.YenAr, StringSplitOptions.None);
            if (pathAr.Length <= 1)
            {
                return false;
            }
            else
            {
                // ドライブ文字を設定する。
                targetPath = string.Concat(pathAr.First(), Const.Yen);

                for (int index = 1; index < pathAr.Length; index++)
                {
                    if (string.IsNullOrWhiteSpace(pathAr[index]))
                    {
                        return false;
                    }
                    else
                    {
                        // 対象フォルダ名のフォルダを検索する。
                        // （正確性を重視したいので、正規化は行わない方法で、フォルダを検索する。）
                        if (!Directory.EnumerateDirectories(targetPath, pathAr[index], SearchOption.TopDirectoryOnly).Any())
                        {
                            return false;
                        }

                        // フォルダ名を追加する。
                        targetPath = string.Concat(targetPath, pathAr[index], Const.Yen);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// ドライブ文字リスト取得処理
        /// </summary>
        /// <returns>ドライブ文字リスト</returns>
        private static List<string> GetDriveLetterList()
        {
            // ドライブ文字リストを返却する。
            // （固定ディスク・リムーバブルドライブ・ネットワークドライブ、かつ使用可能なドライブ文字を設定する。）
            return DriveInfo.GetDrives()
                .Where(Info => Const.AvailableDriveTypeList.Contains(Info.DriveType) && Info.IsReady)
                .Select(Info => Info.Name.Replace(Const.Yen, string.Empty))
                .ToList();
        }

        /// <summary>
        /// 半角数値→全角数値変換処理
        /// </summary>
        /// <param name="narrowNumber">半角数値</param>
        /// <returns>変換した全角数値</returns>
        private static string ConvertNumberWide(int narrowNumber)
        {
            string narrowNumberStr = narrowNumber.ToString(), convertNumber = string.Empty;
            foreach (char number in narrowNumberStr)
            {
                string? value = Const.FullWidthNumberList.ElementAtOrDefault(int.Parse(number.ToString()));
                if (!string.IsNullOrEmpty(value))
                {
                    convertNumber = string.Concat(convertNumber, value);
                }
            }

            return convertNumber;
        }
    }
}
