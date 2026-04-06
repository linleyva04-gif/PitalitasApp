using PitalitasApp.Models;
using Supabase;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Text;

namespace PitalitasApp.Controllers
{

    class Login
    {
        const string sUrl = "https://xtbthnijxrknspqqefcf.supabase.co";
        const string sKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh0YnRobmlqeHJrbnNwcXFlZmNmIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzQ0OTAxMDksImV4cCI6MjA5MDA2NjEwOX0.itpUeFOnlzDL05QCRmcUdW3cbaeZ0C8ILYubVpQOKCQ";

        private static Supabase.Client _supabase;

        public async Task<bool> Conectar()
        {
            try
            {
                var options = new SupabaseOptions
                {
                    AutoRefreshToken = true,
                    AutoConnectRealtime = true
                };

                _supabase = new Supabase.Client(sUrl, sKey, options);

                await _supabase.InitializeAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IniciarSesionConGoogle()
        {
            var redirectTo = "pitalitasapp://";

            //esta envia al usuario si o si a elegir cuenta 
            //var authUrl = $"{sUrl}/auth/v1/authorize?provider=google&redirect_to={redirectTo}&prompt=select_account";

            //esta hace que el usuario si ya tiene cuenta entre en automtico a la app
            // var authUrl = $"{sUrl}/auth/v1/authorize?provider=google&redirect_to={redirectTo}";

            var authUrl = $"{sUrl}/auth/v1/authorize?provider=google&redirect_to={redirectTo}&prompt=select_account";
            try
            {
                var authResult = await Microsoft.Maui.Authentication.WebAuthenticator.Default.AuthenticateAsync(
                    new Uri(authUrl),
                    new Uri(redirectTo));

                var callbackUrl = $"{redirectTo}?{string.Join("&", authResult.Properties.Select(x => $"{x.Key}={x.Value}"))}";

                var session = await _supabase.Auth.GetSessionFromUrl(new Uri(callbackUrl));

                return session != null;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static Supabase.Client GetClient()
        {
            return _supabase;
        }
    }
}
