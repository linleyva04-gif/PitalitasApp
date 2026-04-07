using PitalitasApp.Models;
using Microsoft.Maui.Controls;
using System;

namespace PitalitasApp.Views.Clientes;

public partial class CarritoView : ContentPage
{
    public CarritoView()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Conectamos la lista visual con nuestro almacén global
        ListaCarrito.ItemsSource = CarritoGlobal.Articulos;

        // Calculamos y mostramos el Total
        LabelTotal.Text = $"${CarritoGlobal.TotalCuenta:F2}";
    }

    private async void OnPagar_Clicked(object sender, EventArgs e)
    {
        if (CarritoGlobal.Articulos.Count == 0)
        {
            await DisplayAlert("Carrito vacío", "Agrega unas Pitalitas primero.", "OK");
            return;
        }

        await DisplayAlert("¡Orden Lista!", "Tu pedido está siendo procesado.", "Excelente");
    }
}