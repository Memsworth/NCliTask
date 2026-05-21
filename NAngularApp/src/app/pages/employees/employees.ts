import { Component } from '@angular/core';
export interface Employee {
  messageId: string;
  id: number;
  age: number;
  department: string;
  name: string;
}

@Component({
  selector: 'app-employees',
  imports: [],
  templateUrl: './employees.html',
  styleUrl: './employees.css',
})




export class Employees {}
