export interface SmsLog {
    id: number;
    phoneNumber: string;
    messageText: string;
    direction: string;
    timestamp: string;
    processingStatus: string;
  }

  export enum SmsMessageType {
  VERIFICATION = 'VERIFICATION',
  NOTIFICATION = 'NOTIFICATION',
  VOTING_REMINDER = 'VOTING_REMINDER',
  ELECTION_UPDATE = 'ELECTION_UPDATE',
  CAMPAIGN = 'CAMPAIGN',
  SYSTEM = 'SYSTEM',
  REPLY = 'REPLY'
}

export enum SmsStatus {
  PENDING = 'PENDING',
  SENT = 'SENT',
  DELIVERED = 'DELIVERED',
  FAILED = 'FAILED',
  QUEUED = 'QUEUED',
  PROCESSING = 'PROCESSING',
  UNKNOWN = 'UNKNOWN'
}

export enum SmsDirection {
  INBOUND = 'INBOUND',
  OUTBOUND = 'OUTBOUND'
}

export interface SmsLogFilter {
  phoneNumber?: string;
  messageType?: SmsMessageType;
  status?: SmsStatus;
  direction?: SmsDirection;
  dateFrom?: Date;
  dateTo?: Date;
  provider?: string;
  campaignId?: string;
  searchQuery?: string;
}

export interface SmsLogStats {
  total: number;
  sent: number;
  delivered: number;
  failed: number;
  pending: number;
  totalCost: number;
  deliveryRate: number;
  failureRate: number;
}

export interface SmsLogResponse {
  logs: SmsLog[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
  stats: SmsLogStats;
}

export interface SmsProvider {
  id: string;
  name: string;
  isActive: boolean;
  config: Record<string, any>;
}

export interface SmsCampaign {
  id: string;
  name: string;
  description: string;
  createdAt: Date;
  status: 'ACTIVE' | 'COMPLETED' | 'PAUSED';
}