using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi.BLL.Repository.Implementacion;
using WebApi.BLL.Utilidades;
using WebApi.DAL.Repository.Interfaces;
using WebApi.Entity.DTOs;
using WebApi.Entity.Models;

public class UsuarioServicioTests
{
    private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepositorio;
    private readonly EncriptadorSHA256 _encriptadorSHA256;
    private readonly GeneradorJWT _generadorJWT;
    private readonly Mock<ILogger<UsuarioServicio>> _mockLogger;
    private readonly UsuarioServicio _usuarioServicio;

    public UsuarioServicioTests()
    {
        _mockUsuarioRepositorio = new Mock<IUsuarioRepositorio>();

        // Crear una instancia de IConfiguration con clave Jwt
        var inMemorySettings = new Dictionary<string, string> {
            {"Jwt:key", "F7AE3E05-DA6F-4C1E-AE7F-B817669B56FD"} // Clave de al menos 32 caracteres
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _encriptadorSHA256 = new EncriptadorSHA256(configuration); // Pasar la instancia de IConfiguration
        _generadorJWT = new GeneradorJWT(configuration); // Pasar la instancia de IConfiguration

        _mockLogger = new Mock<ILogger<UsuarioServicio>>();

        _usuarioServicio = new UsuarioServicio(
            _mockUsuarioRepositorio.Object,
            _encriptadorSHA256,
            _generadorJWT,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task RegistrarUsuarioAsync_DebeRetornarError_CuandoCorreoYaExiste()
    {
        var usuarioDto = new UsuarioDTO { Email = "moises@gmail.com", Clave = "123" };
        _mockUsuarioRepositorio.Setup(r => r.EmailExisteAsync(usuarioDto.Email)).ReturnsAsync(true);

        var resultado = await _usuarioServicio.RegistrarUsuarioAsync(usuarioDto);

        Assert.False(resultado.Item1);
        Assert.Equal("Esta dirección de correo electrónico ya está en uso.", resultado.Item2);
    }

    [Fact]
    public async Task RegistrarUsuarioAsync_DebeRegistrarUsuario_CuandoCorreoNoExiste()
    {
        var usuarioDto = new UsuarioDTO { Nombre = "Jaime", Email = "jaime@gmail.com", Clave = "123" };

        // Configurar los mocks para que devuelvan los valores esperados
        _mockUsuarioRepositorio.Setup(r => r.EmailExisteAsync(usuarioDto.Email)).ReturnsAsync(false);
        _mockUsuarioRepositorio.Setup(r => r.GuardarUsuarioAsync(It.IsAny<Usuario>())).ReturnsAsync(true);
        _mockUsuarioRepositorio.Setup(r => r.GuardarCambiosAsync()).ReturnsAsync(true);

        // Llamar al método de servicio
        var resultado = await _usuarioServicio.RegistrarUsuarioAsync(usuarioDto);

        // Verificar los resultados
        Assert.True(resultado.Item1);
        Assert.Equal("Usuario registrado exitosamente.", resultado.Item2);
    }


    [Fact]
    public async Task LoginAsync_DebeRetornarError_CuandoCredencialesInvalidas()
    {
        var loginDto = new LoginDTO { Email = "usuario@example.com", Clave = "1234" };

        _mockUsuarioRepositorio.Setup(r => r.ObtenerPorEmailYPaswordAsync(loginDto.Email, It.IsAny<string>()))
                               .ReturnsAsync((Usuario)null);

        var resultado = await _usuarioServicio.LoginAsync(loginDto);

        Assert.False(resultado.Exito);
        Assert.Equal("Usuario no encontrado", resultado.Mensaje);
        Assert.Null(resultado.Token);
    }

    [Fact]
    public async Task LoginAsync_DebeRetornarToken_CuandoCredencialesValidas()
    {
        var loginDto = new LoginDTO { Email = "jaime@gmail.com", Clave = "123" };
        var usuario = new Usuario { IdUsuario = 1, Nombre = "Jaime", Correo = "jaime@gmail.com" };

        _mockUsuarioRepositorio.Setup(r => r.ObtenerPorEmailYPaswordAsync(loginDto.Email, It.IsAny<string>()))
                               .ReturnsAsync(usuario);

        // Usar una instancia real de GeneradorJWT para generar el token
        var token = _generadorJWT.generarJWT(usuario);

        var resultado = await _usuarioServicio.LoginAsync(loginDto);

        Assert.True(resultado.Exito);
        Assert.Equal("Login exitoso", resultado.Mensaje);
        Assert.Equal(token, resultado.Token);

    }
}

