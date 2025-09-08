import { OptionResponseDto } from './OptionResponseDto';
import { UserPollResponseDto } from './UserPollResponseDto';
export interface PollResponseDto {
    id: string;
    question: string;
    startDate: string;
    endDate: string;
    options?: OptionResponseDto[]; 
    userPolls?: UserPollResponseDto[]; 

    //createdById?: string;  
    //createdByEmail?: string;
}