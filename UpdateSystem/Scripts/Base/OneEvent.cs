
#define NET2_0
namespace System
{
#if NET2_0
    public delegate void Action();
    public delegate void Action<T1, T2>(T1 t1, T2 t2);
#endif
}
