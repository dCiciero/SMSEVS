export interface Candidate {
    id?: number;
    name: string;
    description?: string;
    shortCode?: string;
    party: string;
    phoneNumber: string;
    email: string;
    address: string;
    dateOfBirth: string;
    position: string;
    electionId: number;
    // election: {
    //   id: number;
    //   title: string;
    //   description: string;
    //   startDate: string;
    //   endDate: string;
    //   isActive: boolean;
    // };
    registrationDate?: string;
    isActive: boolean;
  }
  
  export interface CreateCandidateDto {
    name: string;
    description: string;
    shortCode: string;
  }
  
  export interface UpdateCandidateDto {
    name: string;
    description: string;
    shortCode: string;
  }