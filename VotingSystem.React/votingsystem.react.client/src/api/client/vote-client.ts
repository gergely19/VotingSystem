import { postAsJson } from "@/api/client/http";
import { VoteRequestDto } from "../models/VoteRequestDto";
import { VoteResponseDto } from "../models/VoteResponseDto";

export async function vote(voteRequest: VoteRequestDto): Promise<VoteResponseDto> {
  return await postAsJson<VoteRequestDto, VoteResponseDto>("votes", voteRequest);

}