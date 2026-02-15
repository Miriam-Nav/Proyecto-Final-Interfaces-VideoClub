using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ViewModel;

namespace View
{
    /// <summary>
    /// Lógica de interacción para PrincipalView.xaml
    /// </summary>
    public partial class PrincipalView : Window
    {
        private MainWindowViewModel _viewModel = new MainWindowViewModel();
        public PrincipalView()
        {
            InitializeComponent();
            this.DataContext = _viewModel;
            _viewModel.OnAccionSolicitada += ViewModel_OnAccionSolicitada;
        }

        private void ViewModel_OnAccionSolicitada()
        {
            // Según lo que el ViewModel haya guardado en AccionSolicitada, abre una ventana u otra
            switch (_viewModel.AccionSolicitada)
            {
                case "Clientes":
                    ClientesView ventanaClientes = new ClientesView();
                    ventanaClientes.ShowDialog();
                    _viewModel.RefrescarDatos();
                    break;

                case "Catalogo":
                    CatalogoView ventanaCatalogo = new CatalogoView();
                    ventanaCatalogo.ShowDialog();
                    _viewModel.RefrescarDatos();
                    break;

                case "Alquileres":
                    AlquileresView ventanaAlquileres = new AlquileresView();
                    ventanaAlquileres.ShowDialog();
                    _viewModel.RefrescarDatos();
                    break;

                case "Configuracion":
                    ConfiguracionView ventanaConfig = new ConfiguracionView();
                    ventanaConfig.ShowDialog();
                    _viewModel.RefrescarDatos();
                    break;
            }
        }
    }
}
