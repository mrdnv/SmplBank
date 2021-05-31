using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmplBank.Application.Requests
{
    public class InsertUseRequest : IRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
