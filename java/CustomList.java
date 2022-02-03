package example;

import java.util.*;

public interface CustomList<E>
{
  public abstract boolean add (E e);
  public abstract E get (int index);
  public abstract boolean addAll (Collection<? extends E> c);
}