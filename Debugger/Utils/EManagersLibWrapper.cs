namespace ModTools
{
    using ColossalFramework;
    using EManagersLib.API;

    public class EManagersLibWrapper : Singleton<EManagersLibWrapper>
    {
        public PropWrapper Wrapper => PropAPI.Wrapper;
    }
}