using AWX.Resources;

namespace AWX.Cmdlets;

public abstract class RegistrationCommandBase<TResource> : APICmdletBase where TResource: class
{
    protected bool Register(string path, ulong targetId, IResource toResource)
    {
        var targetDescription = $"{typeof(TResource).Name} [{targetId}]";
        var toDescription = $"{toResource.Type} [{toResource.Id}]";

        if (ShouldProcess(targetDescription, $"Register to {toDescription}"))
        {
            var sendData = new Dictionary<string, object>()
            {
                { "id", targetId },
            };
            try
            {
                var result = CreateResource<string>(path, sendData);
                if (result.Response.IsSuccessStatusCode)
                {
                    WriteVerbose($"{targetDescription} is registered to {toDescription}.");
                }
                return result.Response.IsSuccessStatusCode;
            }
            catch (RestAPIException) { }
        }
        return false;
    }

    protected bool Unregister(string path, ulong targetId, IResource fromResource)
    {
        var targetDescription = $"{typeof(TResource).Name} [{targetId}]";
        var fromDescription = $"{fromResource.Type} [{fromResource.Id}]";

        if (ShouldProcess(targetDescription, $"Unregister from {fromDescription}"))
        {
            var sendData = new Dictionary<string, object>()
            {
                { "id", targetId },
                { "disassociate", true }
            };
            try
            {
                var result = CreateResource<string>(path, sendData);
                if (result.Response.IsSuccessStatusCode)
                {
                    WriteVerbose($"{targetDescription} is unregistered from {fromDescription}.");
                }
                return result.Response.IsSuccessStatusCode;
            }
            catch (RestAPIException) { }
        }
        return false;
    }
}
