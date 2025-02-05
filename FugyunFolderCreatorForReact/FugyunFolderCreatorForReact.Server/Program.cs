// アプリケーションビルダーを生成する。
var builder = WebApplication.CreateBuilder(args);

// コントローラーをサービスに追加する。
builder.Services.AddControllers();

// エンドポイントを探索可能にする。
builder.Services.AddEndpointsApiExplorer();

// SwaggerドキュメントとUIを生成するサービスを追加する。
builder.Services.AddSwaggerGen();

// CORSポリシーを設定する。
// （フロントエンド・バックエンドが、別々のポートで動作している場合、CORSエラーが発生することがあるため。）
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder.WithOrigins("https://localhost:65312") // ReactアプリケーションのURLを許可する。（フロントエンドのURLを指定する。）
                          .AllowAnyHeader() // 任意のヘッダーを許可
                          .AllowAnyMethod()); // 任意のHTTPメソッドを許可

    //options.AddPolicy("AllowAll", policy =>
    //{
    //    policy.AllowAnyOrigin()    // すべてのオリジンを許可
    //          .AllowAnyMethod()    // すべてのHTTPメソッドを許可（GET, POST, PUT, DELETE など）
    //          .AllowAnyHeader();   // すべてのヘッダーを許可
    //});
});

// アプリをビルドする。
var app = builder.Build();

// デフォルトのファイル（index.htmlなど）を提供する。
app.UseDefaultFiles();

// 静的ファイル（CSS・JavaScript・画像など）を提供する。
app.UseStaticFiles();

// HTTPリクエストパイプラインを構成する。
if (app.Environment.IsDevelopment())
{
    // 開発環境でのみUseSwaggerとUseSwaggerUIを使用してSwaggerを有効にする。
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPリクエストをHTTPSにリダイレクトする。
app.UseHttpsRedirection();

// CORSポリシーを適用する。
app.UseCors("AllowFrontend");

// 認可ミドルウェアを使用する。
app.UseAuthorization();

// コントローラのエンドポイントをマップする。
app.MapControllers();

// フォールバックファイルを設定する。
// （他のルートが一致しない場合に、/index.htmlにフォールバックする。）
app.MapFallbackToFile("/index.html");

// アプリを実行する。
app.Run();