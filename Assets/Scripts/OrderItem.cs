using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Orders // Contains a List of all 
{
    public List<Order> orders;

    public Orders(List<Order> orders)
    {
        this.orders = orders;
    }

    public Orders()
    {
    }
}

[System.Serializable]
public class Order // Contains 1 foodItem & 1 drinkItem
{
    public OrderItem foodItem;
    public OrderItem drinkItem;

    public Order(OrderItem foodItem, OrderItem drinkItem)
    {
        this.foodItem = foodItem;
        this.drinkItem = drinkItem;
    }

    public Order()
    {
    }
}

[System.Serializable]
public class OrderItem // Food or drink item
{
    public string prefabName;
    public string itemName;

    public OrderItem(string prefabName, string itemName)
    {
        this.prefabName = prefabName;
        this.itemName = itemName;
    }
    public OrderItem()
    {
    }
}
