using System;
using System.Collections.Generic;
using System.Text;

namespace TrackApp.ClientLayer
{
    public static class ClientConsts
    {
        // login and sign up constants 
        public const int PASSWORD_LENGTH = 6;
        public static readonly string PASSWORD_LENGTH_STRING = "Parola de minim " + PASSWORD_LENGTH + " caractere";
        public const string ACCOUNT_CREATED_MESSAGE = "Contul a fost creat!";
        public const string INTERNET_EXCEPTION_MESSAGE = "Nu este internet!";
        public const string DYNAMODB_EXCEPTION_MESSAGE1 = "Probleme cu internet-ul!";
        public const string DYNAMODB_EXCEPTION_MESSAGE2 = "Probleme cu internet-ul/server-ul!";

    }
}
