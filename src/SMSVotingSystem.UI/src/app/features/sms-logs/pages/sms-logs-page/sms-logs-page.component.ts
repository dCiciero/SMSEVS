import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { SmsLog, SmsLogStats, SmsProvider, SmsCampaign, SmsLogFilter, SmsMessageType, SmsStatus, SmsDirection, SmsLogResponse } from '../../models/sms-log.model';
import { SmsLogService } from '../../services/sms-log.service';

@Component({
  selector: 'app-sms-logs-page',
  templateUrl: './sms-logs-page.component.html',
  styleUrl: './sms-logs-page.component.scss'
})
export class SmsLogsPageComponent implements OnInit, OnDestroy {

  logs: SmsLog[] = [];
  stats: SmsLogStats | null = null;
  providers: SmsProvider[] = [];
  campaigns: SmsCampaign[] = [];
  
  loading = false;
  error: string | null = null;
  success: string | null = null;
  
  // Pagination
  currentPage = 1;
  pageSize = 50;
  totalPages = 0;
  totalRecords = 0;
  
  // Filtering
  filterForm!: FormGroup;
  activeFilter: SmsLogFilter = {};
  
  // Selection
  selectedLogs: string[] = [];
  selectAll = false;
  
  // View options
  refreshInterval: any;
  autoRefresh = false;
  viewMode: 'table' | 'cards' = 'table';
  
  // Enums for template
  SmsMessageType = SmsMessageType;
  SmsStatus = SmsStatus;
  SmsDirection = SmsDirection;
  
  private destroy$ = new Subject<void>();


  constructor(
    private smsLogService: SmsLogService,
    private fb: FormBuilder,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.loadInitialData();
    this.setupFormSubscriptions();
    // this.checkRouteMode();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
  }

  private initializeForm(): void {
    this.filterForm = this.fb.group({
      searchQuery: [''],
      phoneNumber: [''],
      messageType: [''],
      status: [''],
      direction: [''],
      provider: [''],
      campaignId: [''],
      dateFrom: [''],
      dateTo: ['']
    });
  }

  private setupFormSubscriptions(): void {
    // Search with debounce
    this.filterForm.get('searchQuery')?.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.applyFilters();
    });

    // Other filters
    this.filterForm.valueChanges.pipe(
      debounceTime(500),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      if (!this.filterForm.get('searchQuery')?.dirty) {
        this.applyFilters();
      }
    });
  }

  getPageEnd(): number {
  return Math.min(this.currentPage * this.pageSize, this.totalRecords);
}

  // private checkRouteMode(): void {
  //   this.route.data.pipe(takeUntil(this.destroy$)).subscribe(data => {
  //     if (data['mode'] === 'details') {
  //       const id = this.route.snapshot.paramMap.get('id');
  //       if (id) {
  //         this.loadLogDetails(id);
  //       }
  //     }
  //   });
  // }

  private loadInitialData(): void {
    this.loadLogs();
    // this.loadStats();
    // this.loadProviders();
    // this.loadCampaigns();
  }

  loadLogs(page: number = this.currentPage): void {
    this.loading = true;
    this.error = null;

    // this.smsLogService.getSmsLogs(page, this.pageSize, this.activeFilter)
    this.smsLogService.getLogs()
      // .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.logs = response;
          // this.currentPage = response.page;
          // this.totalPages = response.totalPages;
          this.totalRecords = response.length;
          // this.stats = response.stats;
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Failed to load SMS logs';
          this.loading = false;
          console.error(error);
        }
      });
  }

  // private loadStats(): void {
  //   this.smsLogService.getSmsStats(this.activeFilter)
  //     .pipe(takeUntil(this.destroy$))
  //     .subscribe({
  //       next: (stats) => this.stats = stats,
  //       error: (error) => console.error('Failed to load stats:', error)
  //     });
  // }

  // private loadProviders(): void {
  //   this.smsLogService.getProviders()
  //     .pipe(takeUntil(this.destroy$))
  //     .subscribe({
  //       next: (providers) => this.providers = providers,
  //       error: (error) => console.error('Failed to load providers:', error)
  //     });
  // }

  // private loadCampaigns(): void {
  //   this.smsLogService.getCampaigns()
  //     .pipe(takeUntil(this.destroy$))
  //     .subscribe({
  //       next: (campaigns) => this.campaigns = campaigns,
  //       error: (error) => console.error('Failed to load campaigns:', error)
  //     });
  // }

  // private loadLogDetails(id: string): void {
  //   this.smsLogService.getSmsLogById(id)
  //     .pipe(takeUntil(this.destroy$))
  //     .subscribe({
  //       next: (log) => {
  //         // Handle log details view
  //         console.log('Log details:', log);
  //       },
  //       error: (error) => {
  //         this.error = 'Failed to load log details';
  //         console.error(error);
  //       }
  //     });
  // }

  applyFilters(): void {
    const formValue = this.filterForm.value;
    this.activeFilter = {
      searchQuery: formValue.searchQuery || undefined,
      phoneNumber: formValue.phoneNumber || undefined,
      messageType: formValue.messageType || undefined,
      status: formValue.status || undefined,
      direction: formValue.direction || undefined,
      provider: formValue.provider || undefined,
      campaignId: formValue.campaignId || undefined,
      dateFrom: formValue.dateFrom ? new Date(formValue.dateFrom) : undefined,
      dateTo: formValue.dateTo ? new Date(formValue.dateTo) : undefined
    };

    this.currentPage = 1;
    this.loadLogs();
    // this.loadStats();
  }

  clearFilters(): void {
    this.filterForm.reset();
    this.activeFilter = {};
    this.currentPage = 1;
    this.loadLogs();
    // this.loadStats();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadLogs(page);
  }

  onPageSizeChange(pageSize: number): void {
    this.pageSize = pageSize;
    this.currentPage = 1;
    this.loadLogs();
  }

  // toggleSelectAll(): void {
  //   this.selectAll = !this.selectAll;
  //   if (this.selectAll) {
  //     this.selectedLogs = this.logs.map(log => log.id);
  //   } else {
  //     this.selectedLogs = [];
  //   }
  // }

  toggleSelectLog(logId: string): void {
    const index = this.selectedLogs.indexOf(logId);
    if (index > -1) {
      this.selectedLogs.splice(index, 1);
    } else {
      this.selectedLogs.push(logId);
    }
    this.selectAll = this.selectedLogs.length === this.logs.length;
  }

  isLogSelected(logId: string): boolean {
    return this.selectedLogs.includes(logId);
  }

  // retrySelected(): void {
  //   if (this.selectedLogs.length === 0) return;

  //   this.loading = true;
  //   this.smsLogService.bulkRetrySms(this.selectedLogs)
  //     .pipe(takeUntil(this.destroy$))
  //     .subscribe({
  //       next: (result) => {
  //         this.success = `Successfully retried ${result.success} SMS. ${result.failed} failed.`;
  //         this.selectedLogs = [];
  //         this.selectAll = false;
  //         this.loadLogs();
  //         this.loading = false;
  //         setTimeout(() => this.success = null, 5000);
  //       },
  //       error: (error) => {
  //         this.error = 'Failed to retry selected SMS';
  //         this.loading = false;
  //         console.error(error);
  //       }
  //     });
  // }

  // retrySingle(logId: string): void {
  //   this.smsLogService.retrySms(logId)
  //     .pipe(takeUntil(this.destroy$))
  //     .subscribe({
  //       next: () => {
  //         this.success = 'SMS retry initiated successfully';
  //         this.loadLogs();
  //         setTimeout(() => this.success = null, 3000);
  //       },
  //       error: (error) => {
  //         this.error = 'Failed to retry SMS';
  //         console.error(error);
  //       }
  //     });
  // }

  // exportLogs(format: 'csv' | 'excel' = 'csv'): void {
  //   this.loading = true;
  //   this.smsLogService.exportLogs(this.activeFilter, format)
  //     .pipe(takeUntil(this.destroy$))
  //     .subscribe({
  //       next: (blob) => {
  //         const url = window.URL.createObjectURL(blob);
  //         const link = document.createElement('a');
  //         link.href = url;
  //         link.download = `sms-logs-${new Date().toISOString().split('T')[0]}.${format}`;
  //         link.click();
  //         window.URL.revokeObjectURL(url);
  //         this.loading = false;
  //         this.success = 'Export completed successfully';
  //         setTimeout(() => this.success = null, 3000);
  //       },
  //       error: (error) => {
  //         this.error = 'Failed to export logs';
  //         this.loading = false;
  //         console.error(error);
  //       }
  //     });
  // }

  toggleAutoRefresh(): void {
    this.autoRefresh = !this.autoRefresh;
    if (this.autoRefresh) {
      this.refreshInterval = setInterval(() => {
        this.loadLogs();
      }, 30000); // Refresh every 30 seconds
    } else {
      if (this.refreshInterval) {
        clearInterval(this.refreshInterval);
      }
    }
  }

  refreshData(): void {
    this.loadLogs();
    // this.loadStats();
  }

  toggleViewMode(): void {
    this.viewMode = this.viewMode === 'table' ? 'cards' : 'table';
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

  getPages(): number[] {
    const pages: number[] = [];
    const startPage = Math.max(1, this.currentPage - 2);
    const endPage = Math.min(this.totalPages, this.currentPage + 2);
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  }
}
