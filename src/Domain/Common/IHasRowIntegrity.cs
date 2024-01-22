namespace PortalCore.Domain.Common
{
    public interface IHasRowIntegrity
    {
        string Hash { set; get; }
    }
}
