// �A�v���P�[�V�����r���_�[�𐶐�����B
var builder = WebApplication.CreateBuilder(args);

// �R���g���[���[���T�[�r�X�ɒǉ�����B
builder.Services.AddControllers();

// �G���h�|�C���g��T���\�ɂ���B
builder.Services.AddEndpointsApiExplorer();

// Swagger�h�L�������g��UI�𐶐�����T�[�r�X��ǉ�����B
builder.Services.AddSwaggerGen();

// CORS�|���V�[��ݒ肷��B
// �i�t�����g�G���h�E�o�b�N�G���h���A�ʁX�̃|�[�g�œ��삵�Ă���ꍇ�ACORS�G���[���������邱�Ƃ����邽�߁B�j
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder.WithOrigins("https://localhost:65312") // React�A�v���P�[�V������URL��������B�i�t�����g�G���h��URL���w�肷��B�j
                          .AllowAnyHeader() // �C�ӂ̃w�b�_�[������
                          .AllowAnyMethod()); // �C�ӂ�HTTP���\�b�h������

    //options.AddPolicy("AllowAll", policy =>
    //{
    //    policy.AllowAnyOrigin()    // ���ׂẴI���W��������
    //          .AllowAnyMethod()    // ���ׂĂ�HTTP���\�b�h�����iGET, POST, PUT, DELETE �Ȃǁj
    //          .AllowAnyHeader();   // ���ׂẴw�b�_�[������
    //});
});

// �A�v�����r���h����B
var app = builder.Build();

// �f�t�H���g�̃t�@�C���iindex.html�Ȃǁj��񋟂���B
app.UseDefaultFiles();

// �ÓI�t�@�C���iCSS�EJavaScript�E�摜�Ȃǁj��񋟂���B
app.UseStaticFiles();

// HTTP���N�G�X�g�p�C�v���C�����\������B
if (app.Environment.IsDevelopment())
{
    // �J�����ł̂�UseSwagger��UseSwaggerUI���g�p����Swagger��L���ɂ���B
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTP���N�G�X�g��HTTPS�Ƀ��_�C���N�g����B
app.UseHttpsRedirection();

// CORS�|���V�[��K�p����B
app.UseCors("AllowFrontend");

// �F�~�h���E�F�A���g�p����B
app.UseAuthorization();

// �R���g���[���̃G���h�|�C���g���}�b�v����B
app.MapControllers();

// �t�H�[���o�b�N�t�@�C����ݒ肷��B
// �i���̃��[�g����v���Ȃ��ꍇ�ɁA/index.html�Ƀt�H�[���o�b�N����B�j
app.MapFallbackToFile("/index.html");

// �A�v�������s����B
app.Run();