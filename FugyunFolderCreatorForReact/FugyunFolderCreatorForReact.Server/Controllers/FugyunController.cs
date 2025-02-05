using FugyunFolderCreatorForReact.Server.Core;
using Microsoft.AspNetCore.Mvc;

namespace FugyunFolderCreatorForReact.Server.Controllers
{
    /// <summary>
    /// ふぎゅんコントローラー
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FugyunController : Controller
    {
        /// <summary>
        /// フォルダ作成リクエスト
        /// </summary>
        public class FolderCreateRequest
        {
            /// <summary>
            /// パス
            /// </summary>
            public string Path { get; set; } = string.Empty;
        }

        /// <summary>
        /// フォルダ削除リクエスト
        /// </summary>
        public class FolderDeleteRequest
        {
            /// <summary>
            /// パス
            /// </summary>
            public string Path { get; set; } = string.Empty;

            /// <summary>
            /// 全削除フラグ
            /// （true：全削除・false：削除）
            /// </summary>
            public bool AllDeleteFlg { get; set; }
        }

        /// <summary>
        /// フォルダ作成処理
        /// </summary>
        /// <returns>メッセージ（true：ブランク・false：エラーメッセージ）</returns>
        [HttpPost("Create")]
        public IActionResult CreateFolder([FromBody] FolderCreateRequest request)
        {
            try
            {
                string resultMsg = new FugyunCore().FolderCreateTran(request.Path);
                if (string.IsNullOrEmpty(resultMsg))
                {
                    return Ok("フォルダが作成されました。");
                }
                else
                {
                    return BadRequest(resultMsg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// フォルダ削除処理
        /// </summary>
        /// <returns>メッセージ（true：ブランク・false：エラーメッセージ）</returns>
        [HttpPost("Delete")]
        public IActionResult DeleteFolder([FromBody] FolderDeleteRequest request)
        {
            try
            {
                string resultMsg = new FugyunCore().FolderDeleteTran(request.Path, request.AllDeleteFlg);
                if (string.IsNullOrEmpty(resultMsg))
                {
                    if (request.AllDeleteFlg)
                    {
                        return Ok("フォルダが全削除されました。");
                    }
                    else
                    {
                        return Ok("フォルダが削除されました。");
                    }
                }
                else
                {
                    return BadRequest(resultMsg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
