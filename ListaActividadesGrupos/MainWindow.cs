using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ListaActividadesGrupos
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<int> _listaAlumnos;
        private List<int> _listaAlumnosRestantes;
        private int _numeroTotalAlumnos = 0;
        private int _numeroAlumnos = 0;
        private int _numeroAlumnas = 0;
        private int _numeroIntegrantes = 1;
        private int _numeroGrupos = 0;
        private bool _mixto = false;
        private static readonly Regex _regex = new Regex("[^0-9]+");

        public MainWindow()
        {
            InitializeComponent();
            tbxNumeroAlumnos.Focus();

        }

        private void inicializarUI()
        {
            _mixto = (bool)ckbMixto.IsChecked;

            if (tbxNumeroGrupos.Text.Replace(" ", "") != String.Empty && tbxNumeroAlumnos.Text.Replace(" ", "") != String.Empty && tbxNumeroAlumnosas.Text.Replace(" ", "") != String.Empty)
            {
                tbxNumeroGrupos.Text = tbxNumeroGrupos.Text.Trim().Replace(" ", "");
                tbxNumeroAlumnos.Text = tbxNumeroAlumnos.Text.Trim().Replace(" ", "");

                _numeroIntegrantes = int.Parse(tbxNumeroGrupos.Text);
                _numeroTotalAlumnos = int.Parse(tbxNumeroAlumnos.Text.Trim().Replace(" ", ""));

                AsignarNumeroAlumnosAs();
            }
            else
                MessageBox.Show("Hay elementos vacíos", "¡ERROR!");

        }

        private void AsignarNumeroAlumnosAs()
        {
            if ((bool)ckbFem.IsChecked)
            {
                tbxNumeroAlumnosas.Text = tbxNumeroAlumnosas.Text.Trim().Replace(" ", "");
                _numeroAlumnas = int.Parse(tbxNumeroAlumnosas.Text);
                _numeroAlumnos = _numeroTotalAlumnos - _numeroAlumnas;

            }

            if ((bool)ckbMasc.IsChecked)
            {
                tbxNumeroAlumnosas.Text = tbxNumeroAlumnosas.Text.Trim().Replace(" ", "");
                _numeroAlumnos = int.Parse(tbxNumeroAlumnosas.Text);
                _numeroAlumnas = _numeroTotalAlumnos - _numeroAlumnos;
            }
        }

        private void TbxNumeroAlumnos_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void BtnGenerarRnd_Click(object sender, RoutedEventArgs e)
        {
            lbxGrupos.Items.Clear();
            inicializarUI();


            if (!GenerosMixtosException())
            {
                CrearGrupos();
            }
            else
            {
                lbxGrupos.Items.Clear();
                inicializarUI();
            }
        }


        private void CrearGrupos()
        {
            _listaAlumnos = new List<int>();

            for (int i = 1; i < _numeroTotalAlumnos + 1; i++)
            {
                _listaAlumnos.Add(i);
            }

            OrdenarListaAlea(_listaAlumnos);
            LlenarLista();

        }

        private void LlenarLista()
        {
            _numeroGrupos = (int)Math.Ceiling((double)(_numeroTotalAlumnos / _numeroIntegrantes));
            int indice = 0;
            int contador = _numeroTotalAlumnos;
            int contadorAlumnosGenero_M = 0;
            int contadorAlumnosGenero_F = 0;
            int contadorAlumnosGeneroPorGrupo = 0;
            List<string>[] grupos = new List<string>[_numeroGrupos];
            bool haContadoM = false;


            for (int i = 0; i < grupos.Length; i++)
            {
                grupos[i] = new List<string>();
            }
            for (int i = 0; i < _numeroTotalAlumnos; i++)
            {
                haContadoM = false;

                if (_numeroAlumnas == 0)
                {
                    grupos[i % _numeroGrupos].Add(_listaAlumnosRestantes[i].ToString() + " - M");
                }

                if (_numeroAlumnos == 0)
                {
                    grupos[i % _numeroGrupos].Add(_listaAlumnosRestantes[i].ToString() + " - F");
                }

                if (_mixto && _numeroAlumnas != 0 && _numeroAlumnos != 0)
                {
                    if (contadorAlumnosGenero_F < _numeroAlumnas)
                    {
                        grupos[i % _numeroGrupos].Add(_listaAlumnosRestantes[i].ToString() + " - F");
                        contadorAlumnosGenero_F++;
                        haContadoM = !haContadoM;
                    }

                    if (contadorAlumnosGenero_M < _numeroAlumnos && !haContadoM)
                    {
                        grupos[i % _numeroGrupos].Add(_listaAlumnosRestantes[i].ToString() + " - M");
                        contadorAlumnosGenero_M++;
                    }
                }

                if (!_mixto && _numeroAlumnas != 0 && _numeroAlumnos != 0)
                {
                    grupos[i % _numeroGrupos].Add(_listaAlumnosRestantes[i].ToString());
                }


            }

            for (int i = 0; i < grupos.Length; i++)
            {
                if (i != 0)
                    lbxGrupos.Items.Add(" ");
                lbxGrupos.Items.Add(" --- Grupo " + (i + 1) + " ---");
                foreach (var item in grupos[i])
                {
                    if (_numeroAlumnas > contadorAlumnosGenero_F && !_mixto)
                    {
                        lbxGrupos.Items.Add(item + " - F");
                        contadorAlumnosGenero_F++;
                    }else if (!_mixto)
                    {
                        lbxGrupos.Items.Add(item + " - M");
                        contadorAlumnosGenero_M++;
                    }

                    if (_mixto)
                    {
                        lbxGrupos.Items.Add(item);
                    }
                }
            }
        }

        private bool GenerosMixtosException()
        {
            if (_numeroAlumnas == _numeroTotalAlumnos && _mixto || _numeroAlumnos == _numeroTotalAlumnos && _mixto)
            {
                MessageBox.Show("No se pueden asignar grupos mixtos con el mismo número de participantes totales del mismo género.", "¡ERROR!");
                return true;
            }

            if (_numeroIntegrantes > _numeroTotalAlumnos || _numeroIntegrantes == 0)
            {
                MessageBox.Show("El número de integrantes es incorrecto.", "¡ERROR!");
                return true;
            }

            if (_numeroAlumnas == 0 && _mixto || _numeroAlumnos == 0 && _mixto || _numeroAlumnas == _numeroTotalAlumnos && _mixto || _numeroAlumnos == _numeroTotalAlumnos && _mixto)
            {
                MessageBox.Show("No se pueden asignar grupos mixtos sin participantes de un género.", "¡ERROR!");
                return true;
            }

            if (_numeroAlumnas > _numeroTotalAlumnos || _numeroAlumnos > _numeroTotalAlumnos)
            {
                MessageBox.Show("El número de alumnos o alumnas por género es incorrecto.", "¡ERROR!");
                return true;
            }

            return false;
        }

        private void CkbMasc_Checked(object sender, RoutedEventArgs e)
        {
            ckbMasc.IsChecked = true;
            ckbFem.IsChecked = false;
        }

        private void CkbFem_Checked(object sender, RoutedEventArgs e)
        {
            ckbFem.IsChecked = true;
            ckbMasc.IsChecked = false;
        }

        private void OrdenarListaAlea(List<int> inputList)
        {
            _listaAlumnosRestantes = new List<int>();
            Random rnd = new Random();
            int indice = 0;

            while (inputList.Count > 0)
            {
                indice = rnd.Next(0, inputList.Count);
                _listaAlumnosRestantes.Add(inputList[indice]);
                inputList.RemoveAt(indice);
            }
        }
    }
}