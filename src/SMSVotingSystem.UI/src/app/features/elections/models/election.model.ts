export interface Election {
    id: number;
    title: string;
    description: string;
    startDate: string;
    endDate: string;
    isActive: boolean;
  }
  
  export interface CreateElectionDto {
    title: string;
    description: string;
    startDate: string;
    endDate: string;
  }
  
  export interface UpdateElectionDto {
    title: string;
    description: string;
    startDate: string;
    endDate: string;
  }