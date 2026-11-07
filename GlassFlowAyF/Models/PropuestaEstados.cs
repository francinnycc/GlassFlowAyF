namespace GlassFlowAyF.Models
{
    public static class EstadosPropuesta
    {
        public const string AprobadoPorElCliente =
            "Aprobado por el Cliente";

        public const string SolicitudDeModificacion =
            "Solicitud de Modificación";

        public const string PropuestaRechazadaPorElCliente =
            "Propuesta Rechazada por el Cliente";

        public const string AprobadoLegado = "Aprobado";
    }

    public static class TiposDecisionPropuesta
    {
        public const string Aprobar = "Aprobada";

        public const string SolicitarModificacion =
            "SolicitudModificacion";

        public const string Rechazar = "Rechazada";
    }

    public static class TiposNotificacion
    {
        public const string Aprobacion = "Aprobacion";

        public const string SolicitudModificacion =
            "SolicitudModificacion";

        public const string Rechazo = "Rechazo";
    }
}
