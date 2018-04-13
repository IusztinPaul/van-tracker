using System;


namespace TrackApp.ClientLayer.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
