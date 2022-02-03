package example;

import java.util.*;

public class CustomListConsumer<E>
{
    CustomList<E> list;

    public CustomListConsumer (CustomList<E> list)
    {
        this.list = list;
    }

    public boolean add (E e)
    {
        return list.add (e);
    }

    public boolean addAll (Collection<? extends E> c)
    {
        return list.addAll (c);
    }

    public E get (int index)
    {
        return list.get (index);
    }
}