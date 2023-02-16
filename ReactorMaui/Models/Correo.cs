﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Models
{
    public static class Correo
    {
        public static bool Enviar(string Correo, string Asunto, string Codigo)
        {
            string From = "privadasverificacion500@hotmail.com";
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(From, "Codigo de verificación"),
                    Subject = Asunto,
                    IsBodyHtml = true,
                    Body = $"<center><h1>{ Codigo }</h1></center>"
                };

                mail.To.Add(Correo);

                SmtpClient client = new SmtpClient("smtp.office365.com", 587)
                {
                    Credentials = new NetworkCredential(From, "Mailo100"),
                    EnableSsl = true
                };
           
                client.Send(mail);

                return true;

            }catch(Exception ex)
            {
                return false;
            }
        }
    }
}
