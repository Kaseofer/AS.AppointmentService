using System.Text;

namespace AS.AppointmentService.Application.Common
{
    public static class ExceptionHelper
    {
        public static string GetFullMessage(Exception ex)
        {
            var mensaje = new StringBuilder(ex.Message);

            var inner = ex.InnerException;
            while (inner != null)
            {
                mensaje.Append(" | Detalle: ").Append(inner.Message);
                inner = inner.InnerException;
            }

            return mensaje.ToString();
        }
    }

}
