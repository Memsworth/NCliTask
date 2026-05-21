import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

export interface EmployeeData {
  messageId: string;
  id: number;
  age: number;
  department: string;
  name: string;
}

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  private hubConnection!: signalR.HubConnection;
  private employeeSubject = new BehaviorSubject<EmployeeData | null>(null);
  employees = this.employeeSubject.asObservable();


    startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/messageHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.error('SignalR error:', err));

    this.registerListeners();
  }

    private registerListeners(): void {
    this.hubConnection.on('EmployeeData', (data: EmployeeData) => {
      console.log('Received:', data);
      this.employeeSubject.next(data);
    });
  }

    stopConnection(): void {
    this.hubConnection?.stop();
  }
}
