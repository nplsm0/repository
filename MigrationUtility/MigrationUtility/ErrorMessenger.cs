using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility
{
    /// <summary>
    /// Представляет собой отправитель сообщений источнику-валдельцу
    /// </summary>
    internal class ErrorMessenger
    {
        public void Send(string message) => ErrorSended?.Invoke(this, message);

        public event EventHandler<string> ErrorSended;
    }
}
