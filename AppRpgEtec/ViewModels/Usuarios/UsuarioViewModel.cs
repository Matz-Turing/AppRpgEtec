using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;
using AppRpgEtec.Views;
using AppRpgEtec.Views.Personagens;
using AppRpgEtec.Views.Usuarios;


namespace AppRpgEtec.ViewModels.Usuarios
{
    public class UsuarioViewModel : BaseViewModel
    {
        private UsuarioService uService;
        public ICommand AutenticarCommand { get; set; }
        public ICommand RegistrarCommand { get; set; }
        public ICommand DirecionarCadastroCommand { get; set; }

        public UsuarioViewModel()
        {
            uService = new UsuarioService();
            InicializarCommands();
        }


        public void InicializarCommands()
        {
            RegistrarCommand = new Command(async () => await RegistrarUsuario());
            AutenticarCommand = new Command(async () => await AutenticarUsuario());
            DirecionarCadastroCommand = new Command(async () => await DirecionarParaCadastro());
        }
        #region AtributosPropriedades
            // As propriedades serão chamadas na View futuramente

        private string _login = string.Empty; // Usando _ para indicar variável privada

        public string Login
        {
            get { return _login; }
            set
            {
                if (_login != value) // Verifica se o valor mudou antes de atualizar
                {
                    _login = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _senha = string.Empty; // Usando _ para indicar variável privada

        public string Senha
        {
            get { return _senha; }
            set
            {
                if (_senha != value) // Verifica se o valor mudou antes de atualizar
                {
                    _senha = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion


        public async Task AutenticarUsuario()//Método para autenticar um usuário
        {
            try
            {
                Usuario u = new Usuario();
                u.Username = Login;
                u.PasswordString = Senha;
                Usuario uAutenticado = await uService.PostAutenticarUsuarioAsync(u);
                if (!string.IsNullOrEmpty(uAutenticado.Token))
                {
                    string mensagem = $"Bem-vindo(a) {uAutenticado.Username}.";

                    //Guardando dados do usuário para uso futuro 
                    Preferences.Set("UsuarioId", uAutenticado.Id);
                    Preferences.Set("UsuarioUsername", uAutenticado.Username);
                    Preferences.Set("UsuarioPerfil", uAutenticado.Perfil);
                    Preferences.Set("UsuarioToken", uAutenticado.Token);

                    await Application.Current.MainPage
                    .DisplayAlert("Informação", mensagem, "Ok");

                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await Application.Current.MainPage
                    .DisplayAlert("Informação", "Dados incorretos :(", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                .DisplayAlert("Informação", ex.Message + " Detalhes: " + ex.InnerException, "Ok");

            }
        }

        public async Task DirecionarParaCadastro()//Método para exibição da view de Cadastro
        {
            try
            {
                await Application.Current.MainPage.
                Navigation.PushAsync(new CadastroView());
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                .DisplayAlert("Informação", ex.Message + "Detalhes: "+ ex. InnerException, "Ok");
            }
        }

        #region Métodos
        public async Task RegistrarUsuario()//Método para registrar um usuário
        {
            try
            {
                Usuario u = new Usuario();
                u.Username = Login; 
                u.PasswordString = Senha;
                Usuario uRegistrado = await uService.PostRegistrarUsuarioAsync(u);
                if (uRegistrado.Id != 0)
                {
                    string mensagem = $"Usuário Id {uRegistrado.Id} registrado com sucesso."; await Application.Current.MainPage.DisplayAlert("Informação", mensagem, "Ok");
                    await Application.Current.MainPage
                    .Navigation.PopAsync(); //Remove a página da pilha de visualização
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                .DisplayAlert("Informação", ex.Message ,"Detalhes: ex.InnerException", "Ok");
                #endregion
            }
        }

    }
}
