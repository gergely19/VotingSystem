import { get } from "@/api/client/http";
import { PollResponseDto } from "@/api/models/PollResponseDto";

export async function getPolls(count?: number): Promise<PollResponseDto[]> {
    return await get<PollResponseDto[]>("polls", count ? { count: count.toString() } : undefined);
}

export async function getActivePolls(count?: number): Promise<PollResponseDto[]> {
    return await get<PollResponseDto[]>("polls/actives", count ? { count: count.toString() } : undefined);
}

export async function getClosedPolls(count?: number): Promise<PollResponseDto[]> {
    return await get<PollResponseDto[]>("polls/closed", count ? { count: count.toString() } : undefined);
}

export async function getPoll(id: number): Promise<PollResponseDto> {
    return await get<PollResponseDto>(`polls/${id}`);
}