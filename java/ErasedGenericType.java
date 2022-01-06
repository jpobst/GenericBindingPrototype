package example;

public class ErasedGenericType<T>
{
  public final void TestPerformance (T obj, int count)
  {
    for (int i = 0; i < count; i++)
      PerformanceMethod (obj);
  }
  
  public void PerformanceMethod (T obj) {  }
}