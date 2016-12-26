using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zen.Tracker.Client.Services
{
    public interface IAuthenticate
    {
        Task<bool> AuthenticateAsync();
    }
}
