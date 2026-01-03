namespace NeoClinic.Api.Endpoints;

public static class EndpointGroupBase
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();
        app.MapAppointmentEndpoints();
        app.MapDoctorEndpoints();
        app.MapServiceEndpoints();
        app.MapMediaFileEndpoints();
        app.MapContactMessageEndpoints();
        return app;
    }
}
