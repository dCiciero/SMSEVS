export interface Voter {
    id: number;
    phoneNumber: string;
    name: string;
    isRegistered: boolean;
    registrationDate: string;
    lastVoted: string | null;
  }
  
  export interface CreateVoterDto {
    phoneNumber: string;
    name: string;
  }
  
  export interface UpdateVoterDto {
    name: string;
    isRegistered: boolean;
  }