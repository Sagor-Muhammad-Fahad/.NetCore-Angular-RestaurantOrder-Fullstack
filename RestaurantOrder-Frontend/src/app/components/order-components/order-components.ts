import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { OrderService } from '../../services/order-service';
import { single } from 'rxjs';
import { Order } from '../../models/order';
import { OrderItem } from '../../models/order-item';
import { MenuItem } from '../../models/menu-item';
import { RestaurantTable } from '../../models/restaurant-table';

@Component({
  imports: [CommonModule, ReactiveFormsModule],
  selector: 'app-order-components',
  styleUrl: './order-components.css',
  templateUrl: './order-components.html',
})
export class OrderComponents implements OnInit {
  onSubmit(): void {
    if (this.orderForm.invalid) {
      this.orderForm.markAllAsTouched();
      return;
    }

    const formValue = this.orderForm.value;
    const editId = this.editOrderId();

    const orderPayLoad: Order = {
      customerName: formValue.customerName,
      mobileNo: formValue.mobileNo,
      orderDate: formValue.orderDate,
      isPaid: formValue.isPaid,
      totalAmount: Number(formValue.totalAmount),
      restaurantTableId: Number(formValue.restaurantTableId),
      receiptImageUrl: null,
      orderItems: formValue.orderItems.map((it: any) => ({
        menuItemId: Number(it.menuItemId),
        quantity: Number(it.quantity),
      })),
    };
    this.isSaving.set(true);

    if (this.isEditMode() && editId !== null) {
      orderPayLoad.orderId = editId;
      this.orderService.updateOrder(editId, orderPayLoad).subscribe({
        next: () => this.handleFileUpload(editId),
        error: (err) => {
          console.error('Error in updating order:' + err);
          this.isSaving.set(false);
        },
      });
    } else {
      this.orderService.saveOrder(orderPayLoad).subscribe({
        next: (created) => this.handleFileUpload(created.orderId!),
        error: (err) => {
          console.error('Error' + err);
          this.isSaving.set(false);
        },
      });
    }
  }
  handleFileUpload(orderId: number): void {
    const file = this.selectedFile();
    if (!file) {
      this.finishAndReset();
      return;
    }
    this.orderService.uploadImage(orderId, file).subscribe({
      next: () => this.finishAndReset(),
      error: (err) => {
        console.error('Receipt Upload Failed' + err);
        this.finishAndReset();
      },
    });
  }
  finishAndReset(): void {
    this.isSaving.set(false);
    this.loadOrder();
    this.resetForm();
  }
  deleteOrder(id: number): void {
    if (confirm('Are you Sure you want to Delete This Order')) {
      this.orderService.deleteOrder(id).subscribe({
        next: () => this.loadOrder(),
        error: (err) => console.error(err),
      });
    }
  }
  editOrder(order: Order): void {
    this.isEditMode.set(true);
    this.editOrderId.set(order.orderId!);
    this.selectedFile.set(null);
    this.receiptPreview.set(
      order.receiptImageUrl ? `${this.serverUrl}${order.receiptImageUrl}` : null,
    );

    let orderDateFormatted = '';
    if (order.orderDate) {
      const d = new Date(order.orderDate);
      if (!isNaN(d.getTime())) {
        orderDateFormatted = d.toDateString().split('T')[0];
      }
    }
    this.orderForm.patchValue({
      customerName: order.customerName,
      mobileNo: order.mobileNo,
      orderDate: orderDateFormatted,
      isPaid: order.isPaid,
      totalAmount: order.totalAmount,
      restaurantTableId: order.restaurantTableId,
    });

    this.orderItem.clear();
    if (order.orderItems?.length) {
      order.orderItems.forEach((item) => {
        this.addOrderItem(item.menuItemId, item.quantity);
      });
    }
  }
  resetForm() {
    this.isEditMode.set(false);
    this.editOrderId.set(null);
    this.selectedFile.set(null);
    this.receiptPreview.set(null);
    this.orderForm.reset({
      isPaid: false,
      totalAmount: 0,
      restaurantTableId: 0,
    });
    this.orderItem.clear();
  }
  removeOrderItem(index: number): void {
    this.orderItem.removeAt(index);
  }
  addOrderItem(menuItemId: number = 0, quantity: number = 1): void {
    this.orderItem.push(this.newOrderItem(menuItemId, quantity));
  }
  newOrderItem(menuItemId: number, quantity: number): FormGroup {
    return this.formBuilder.group({
      menuItemId: [menuItemId, [Validators.required, Validators.min(1)]],
      quantity: [quantity, [Validators.required, Validators.min(1)]],
    });
  }
  onFileSelected(event: Event): void {
    const input = event?.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) {
      this.selectedFile.set(file);
      const reader = new FileReader();
      reader.onload = () => {
        this.receiptPreview.set(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  }
  private formBuilder = inject(FormBuilder);
  private orderService = inject(OrderService);

  order = signal<Order[]>([]);
  menuItems = signal<MenuItem[]>([]);
  tables = signal<RestaurantTable[]>([]);
  selectedFile = signal<File | null>(null);
  receiptPreview = signal<string | null>(null);
  isEditMode = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  editOrderId = signal<number | null>(null);

  orderForm!: FormGroup;

  serverUrl = 'http://localhost:5095';

  ngOnInit(): void {
    this.loadOrder();
    this.loadTables();
    this.loadMenuItem();
    this.initForm();
  }

  loadOrder(): void {
    this.orderService.getOrdersWithDetails().subscribe({
      next: (data) => this.order.set(data),
      error: (err) => console.error(err),
    });
  }

  loadMenuItem(): void {
    this.orderService.getMenuItem().subscribe({
      next: (data) => this.menuItems.set(data),
      error: (err) => console.error(err),
    });
  }

  loadTables(): void {
    this.orderService.getRestaurantTables().subscribe({
      next: (data) => this.tables.set(data),
      error: (err) => console.error(err),
    });
  }

  initForm(): void {
    this.orderForm = this.formBuilder.group({
      customerName: ['', [Validators.required]],
      mobileNo: ['', [Validators.required]],
      orderDate: ['', [Validators.required]],
      isPaid: [false],
      totalAmount: [0, [Validators.required, Validators.min(0)]],
      restaurantTableId: [0, [Validators.required, Validators.min(1)]],
      orderItems: this.formBuilder.array([]),
    });
  }

  get orderItem(): FormArray {
    return this.orderForm.get('orderItems') as FormArray;
  }
}
