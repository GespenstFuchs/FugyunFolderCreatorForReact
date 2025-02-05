import { useEffect, useState, useRef } from 'react';
import ConfirmDialog from "./ConfirmDialog";
import MessageDialog from "./MessageDialog";
import infomationIcon from './icons/Infomation.png';
import errorIcon from './icons/Error.png';
import questionIcon from './icons/Question.png';
import './App.css';

function App() {
    // 入力されたパステキストを保持する。
    const [pathText, setPathText] = useState<string>("");

    // ローディング状態を保持する。
    const [isLoading, setIsLoading] = useState<boolean>(false);

    // 確認ダイアログ関連
    const [isDialogOpen, setDialogOpen] = useState<boolean>(false);
    const [dialogMessage, setDialogMessage] = useState<string>("");
    const [dialogCallback, setDialogCallback] = useState<() => void>(() => { });

    // メッセージダイアログ関連
    const [isMessageDialogOpen, setMessageDialogOpen] = useState<boolean>(false);
    const [messageDialogMessage, setMessageDialogMessage] = useState<string>("");
    const [messageDialogIcon, setMessageDialogIcon] = useState<string>("");

    // textareaにフォーカスを当てるためのref作成する。
    const textAreaRef = useRef<HTMLTextAreaElement>(null);

    // 全削除フラグ（true：全削除・false：削除）
    const allDeleteFlg = useRef<boolean>(false);

    // APIURLを取得・保持する。
    const apiUrl = import.meta.env.VITE_REACT_APP_API_URL;

    // 初期表示処理
    useEffect(() => {
        // textareaにフォーカス設定する。
        // （但し、ブラウザの仕様で設定可能なブラウザ・設定不可のブラウザがある（エラーにはならない）。）
        textAreaRef.current?.focus();
    }, []);

    // フォルダ作成処理
    const handleCreateFolder = async () => {
        // ローディングを表示する。
        setIsLoading(true);

        try {
            const response = await fetch(apiUrl + "/Fugyun/Create", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ path: pathText })
            });

            const result = await response.text();

            if (response.ok) {
                showMessageDialog(result, infomationIcon);
            } else {
                showMessageDialog(result, errorIcon);
            }
        } catch (error) {
            const errorMessage = error instanceof Error ? error.message : String(error);
            showMessageDialog(errorMessage, errorIcon);
        } finally {
            // ローディングを非表示する。
            setIsLoading(false);
        }
    };

    // フォルダ削除処理
    const handleDeleteFolder = async () => {
        // ローディングを表示する。
        setIsLoading(true);

        try {
            const response = await fetch(apiUrl + "/Fugyun/Delete", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ path: pathText, allDeleteFlg: allDeleteFlg.current })
            });

            const result = await response.text();

            if (response.ok) {
                showMessageDialog(result, infomationIcon);
            } else {
                showMessageDialog(result, errorIcon);
            }
        } catch (error) {
            const errorMessage = error instanceof Error ? error.message : String(error);
            showMessageDialog(errorMessage, errorIcon);
        } finally {
            // ローディングを非表示する。
            setIsLoading(false);
        }
    };

    // 入力したパスをクリア処理
    const handleClear = () => setPathText("");

    // 確認ダイアログ表示処理
    const showConfirmDialog = (message: string, callback: () => void) => {
        setDialogMessage(message);
        setDialogCallback(() => callback);
        setDialogOpen(true);
    }

    // 確認ダイアログ・はいボタン押下処理
    const handleConfirmDialogYes = () => {
        // 設定されたコールバックを実行する。
        if (dialogCallback) {
            dialogCallback();
        }
        setDialogOpen(false);
    };

    // 確認ダイアログ・いいえボタン押下処理
    const handleConfirmDialogNo = () => {
        setDialogOpen(false);
    };

    // メッセージボックス表示処理
    const showMessageDialog = (message: string, messageDialogIcon: string) => {
        setMessageDialogMessage(message);
        setMessageDialogIcon(messageDialogIcon);
        setMessageDialogOpen(true);
    };

    // メッセージボックス・ＯＫボタン押下処理
    const handleMessageDialogClose = () => {
        setMessageDialogOpen(false);
    };

    // フォルダ作成ボタン押下処理
    const onFolderCreateClick = () => {
        showConfirmDialog("入力されたパスでフォルダを作成してよろしいですか？", handleCreateFolder);
    };

    // フォルダ削除ボタン押下処理
    const onFolderDeleteClick = () => {
        allDeleteFlg.current = false;
        showConfirmDialog(`入力されたパスのフォルダを削除してよろしいですか？\n（フォルダ内にファイルが存在する場合、ファイルごと削除します。）`, handleDeleteFolder);
    };

    // フォルダ全削除ボタン押下処理
    const onFolderAllDeleteClick = () => {
        allDeleteFlg.current = true;
        showConfirmDialog(`入力されたパスの全フォルダを削除してよろしいですか？\n（フォルダ内にファイルが存在する場合、ファイルごと削除します。）`, handleDeleteFolder);
    };

    return (
        <div style={{ padding: '20px', height: '94vh', display: 'flex', flexDirection: 'column' }}>
            {/* 説明ラベル */}
            <div className="labelStyle">
                フォルダを作成したい場合：フォルダパス入力後、【フォルダ作成】ボタンを押下して下さい。<br />
                フォルダを削除したい場合：フォルダパス入力後、【フォルダ削除】ボタンを押下して下さい。<br />
                複数のフォルダを削除したい場合：フォルダパス入力後、【フォルダ全削除】ボタンを押下して下さい。<br />
                １行最大２４０文字がパスとして入力可能で、１パスとします。（改行することで、複数のパスを入力出来ます。）
            </div>

            {/* ボタン群 */}
            <div style={{ margin: '20px 0', display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
                <button id="FolderCreateButton" className="buttonStyle" onClick={onFolderCreateClick}>フォルダ作成</button>
                <button id="FolderDeleteButton" className="buttonStyle" onClick={onFolderDeleteClick}>フォルダ削除</button>
                <button id="FolderAllDeleteButton" className="buttonStyle" onClick={onFolderAllDeleteClick}>フォルダ全削除</button>
                <button id="PathClearButton" className="buttonStyle" onClick={handleClear}>入力したパスをクリア</button>
            </div>

            {/* 複数行テキストボックス：ウィンドウサイズに合わせてリサイズ可能 */}
            {/* テキストエリア部分を Flex アイテムにして、残りの領域を占有させる */}
            <div style={{ flex: 1 }}>
                <textarea
                    ref={textAreaRef}
                    value={pathText}
                    onChange={(e) => setPathText(e.target.value)}
                    wrap="off"
                    placeholder="フォルダパスを入力してください。"
                    style={{
                        width: '100%',
                        height: '100%',
                        resize: 'both',
                        overflowX: 'scroll',
                        overflowY: 'scroll',
                        fontFamily: 'Yu Gothic UI',
                        fontSize: '24px',
                        backgroundColor: '#E6E6FA'
                    }}
                />
            </div>

            {/* 確認ダイアログ */}
            {isDialogOpen && (
                <ConfirmDialog
                    message={dialogMessage}
                    onConfirm={handleConfirmDialogYes}
                    onCancel={handleConfirmDialogNo}
                    iconSrc={questionIcon}
                />
            )}

            {/* メッセージダイアログ */}
            {isMessageDialogOpen && (
                <MessageDialog
                    message={messageDialogMessage}
                    onClose={handleMessageDialogClose}
                    iconSrc={messageDialogIcon}
                />
            )}

            {/* ローディングオーバーレイ */}
            {isLoading && (
                <div className="loading-overlay">
                    <div className="spinner">処理中...</div>
                </div>
            )}
        </div>
    );
}

export default App;