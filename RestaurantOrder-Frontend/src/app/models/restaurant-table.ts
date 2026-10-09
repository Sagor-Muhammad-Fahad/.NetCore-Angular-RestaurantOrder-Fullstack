import { Order } from './order';

export interface RestaurantTable {
  restaurantTableId?: number;
  tableNumber: string;
  capacity: number;
  orders?: Order[];
}
