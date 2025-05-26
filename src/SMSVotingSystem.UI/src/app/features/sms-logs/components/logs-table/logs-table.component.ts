import { Component, EventEmitter, Input, Output } from '@angular/core';
import { SmsDirection, SmsLog, SmsMessageType, SmsStatus } from '../../models/sms-log.model';
import { SmsLogService } from '../../services/sms-log.service';

@Component({
  selector: 'app-logs-table',
  templateUrl: './logs-table.component.html',
  styleUrl: './logs-table.component.scss'
})
export class LogsTableComponent {
  @Input() logs: SmsLog[] = [];
  @Input() selectedLogs: string[] = [];
  @Input() selectAll = false;
  @Output() toggleSelectAll = new EventEmitter<void>();
  @Output() toggleSelectLog = new EventEmitter<string>();
  @Output() retryLog = new EventEmitter<string>();

  // Sorting
  sortColumn: string = 'timestamp';
  sortDirection: 'asc' | 'desc' = 'desc';

  // Enums for template
  SmsStatus = SmsStatus;
  SmsDirection = SmsDirection;
  SmsMessageType = SmsMessageType;

  constructor(private smsLogService: SmsLogService) {}

  // Track by function for ngFor performance
  trackByLogId(index: number, log: SmsLog): string {
    return String(log.id);
  }

  onSelectAllChange(): void {
    this.toggleSelectAll.emit();
  }

  onSelectLogChange(logId: number): void {
    this.toggleSelectLog.emit(String(logId));
  }

  onRetryLog(logId: string): void {
    this.retryLog.emit(logId);
  }

  isLogSelected(logId: number): boolean {
    return this.selectedLogs.includes(String(logId));
  }

  sort(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }

    this.logs.sort((a, b) => {
      let aValue: any = this.getColumnValue(a, column);
      let bValue: any = this.getColumnValue(b, column);

      // Handle different data types
      if (aValue instanceof Date && bValue instanceof Date) {
        aValue = aValue.getTime();
        bValue = bValue.getTime();
      } else if (typeof aValue === 'string' && typeof bValue === 'string') {
        aValue = aValue.toLowerCase();
        bValue = bValue.toLowerCase();
      }

      let comparison = 0;
      if (aValue > bValue) {
        comparison = 1;
      } else if (aValue < bValue) {
        comparison = -1;
      }

      return this.sortDirection === 'desc' ? comparison * -1 : comparison;
    });
  }

  private getColumnValue(log: SmsLog, column: string): any {
    switch (column) {
      case 'timestamp':
        return log.timestamp;
      case 'phoneNumber':
        return log.phoneNumber;
      case 'message':
        return log.messageText;
      // case 'status':
      //   return log.status;
      // case 'messageType':
      //   return log.messageType;
      // case 'direction':
      //   return log.direction;
      // case 'provider':
      //   return log.provider;
      // case 'cost':
      //   return log.cost || 0;
      // case 'retryCount':
      //   return log.retryCount;
      default:
        return '';
    }
  }

  getSortIcon(column: string): string {
    if (this.sortColumn !== column) {
      return 'bi bi-sort text-muted';
    }
    return this.sortDirection === 'asc' ? 'bi bi-sort-up' : 'bi bi-sort-down';
  }

  // Helper methods for template
  // getMessageTypeDisplayName(type: SmsMessageType): string {
  //   return this.smsLogService.getMessageTypeDisplayName(type);
  // }

  // getStatusDisplayName(status: SmsStatus): string {
  //   return this.smsLogService.getStatusDisplayName(status);
  // }

  // getStatusBadgeClass(status: SmsStatus): string {
  //   return this.smsLogService.getStatusBadgeClass(status);
  // }

  formatPhoneNumber(phone: string): string {
    if (phone.length === 10) {
      return `(${phone.substring(0, 3)}) ${phone.substring(3, 6)}-${phone.substring(6)}`;
    }
    return phone;
  }

  truncateMessage(message: string, maxLength: number = 50): string {
    if (message.length <= maxLength) {
      return message;
    }
    return message.substring(0, maxLength) + '...';
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  getDirectionIcon(direction: string): string {
    return direction.toUpperCase() === SmsDirection.INBOUND ? 'bi bi-arrow-down text-success' : 'bi bi-arrow-up text-primary';
  }

  getDirectionText(direction: string): string {
    return direction.toUpperCase() === SmsDirection.INBOUND ? 'Inbound' : 'Outbound';
  }

  canRetry(status: SmsStatus): boolean {
    return status === SmsStatus.FAILED;
  }

  getRowClass(log: SmsLog): string {
    const classes = ['log-row'];
    
    if (this.isLogSelected(log.id)) {
      classes.push('selected');
    }

    // switch (log.status) {
    //   case SmsStatus.FAILED:
    //     classes.push('row-failed');
    //     break;
    //   case SmsStatus.DELIVERED:
    //     classes.push('row-delivered');
    //     break;
    //   case SmsStatus.PENDING:
    //     classes.push('row-pending');
    //     break;
    // }

    return classes.join(' ');
  }

  formatTimestamp(timestamp: string): string {
    const date = new Date(timestamp);
    const now = new Date();
    const diffInHours = (now.getTime() - date.getTime()) / (1000 * 60 * 60);

    if (diffInHours < 24) {
      return date.toLocaleTimeString();
    } else if (diffInHours < 168) { // 1 week
      return date.toLocaleDateString(undefined, { weekday: 'short', hour: '2-digit', minute: '2-digit' });
    } else {
      return date.toLocaleDateString();
    }
  }

//   getDeliveryTime(log: SmsLog): string {
//     if (!log.deliveredAt || !log.timestamp) {
//       return '-';
//     }

//     const sentTime = new Date(log.timestamp).getTime();
//     const deliveredTime = new Date(log.deliveredAt).getTime();
//     const diffInSeconds = (deliveredTime - sentTime) / 1000;

//     if (diffInSeconds < 60) {
//       return `${Math.round(diffInSeconds)}s`;
//     } else if (diffInSeconds < 3600) {
//       return `${Math.round(diffInSeconds / 60)}m`;
//     } else {
//       return `${Math.round(diffInSeconds / 3600)}h`;
//     }
//   }
}
