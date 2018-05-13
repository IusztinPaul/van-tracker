using System;
using System.Collections.Generic;
using System.Text;

namespace TrackApp.ClientLayer.Exceptions
{
    public class TypeException : Exception
    {
        public TypeException(string message) : base(message) { }
    }
}
