import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Supplier } from '../../../classes/Supplier';
import { SupplierService } from '../../../services/supplier.service';

@Component({
  selector: 'app-add-modify-supplier',
  templateUrl: './add-modify-supplier.component.html',
  styleUrls: ['./add-modify-supplier.component.css']
})
export class AddModifySupplierComponent implements OnInit {


  constructor(private service: SupplierService, private router: Router, private toastr: ToastrService) { }

  item: Supplier;

  @Input() sup: Supplier;  
  name: string;
  address: string;
  multiplier: number;
  supplierId: number;

  swap_enabled_name: boolean;
  swap_enabled_address: boolean;
  swap_enabled_multiplier: boolean;

  ngOnInit(): void {
    try {
      var _item_json = localStorage.getItem('item');
      this.item = JSON.parse(_item_json);     
      localStorage.removeItem('item');
    } catch (err) {
      this.item = null;
    }
    this.swap_enabled_name = true;
    this.swap_enabled_multiplier = true;
    this.swap_enabled_address = true;
  }
  addSupplier() {
    let val: Supplier;
    val = { supplierId: this.supplierId, name: this.name, address: this.address, multiplier: this.multiplier };   
    this.service.create(val).subscribe( () => { this.router.navigate(['/supplier']); }, (error) => {
      this.toastr.error(error.error, "Error");
    });   
  }

  updateSupplier() {
    let val: Supplier;
    val = { supplierId: this.item.supplierId, name: this.name, address: this.address, multiplier: this.multiplier };
   
    this.service.update(val.supplierId, val).subscribe( () => { this.router.navigate(['/supplier']); }, (error) => {
      this.toastr.error(error.error, "Error");
    });
  }
  cancel() {
    this.router.navigate(['/supplier']);
  }

  swapToValueFromPlaceHolder_name() {
    if (this.swap_enabled_name) {
      this.name = this.item.name;
      this.swap_enabled_name = !this.swap_enabled_name;
    }
  
  }
  swapToValueFromPlaceHolder_address() {
    if (this.swap_enabled_address) {
      this.address = this.item.address;
      this.swap_enabled_address = !this.swap_enabled_address;
    }
  
  }
  swapToValueFromPlaceHolder_multiplier() {
    if (this.swap_enabled_multiplier) {
      this.multiplier = this.item.multiplier;
      this.swap_enabled_multiplier = !this.swap_enabled_multiplier;
    }
   
  }
}
