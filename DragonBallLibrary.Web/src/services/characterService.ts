import axios from 'axios';
import { DragonBallCharacter, CreateCharacterRequest, UpdateCharacterRequest } from '../types/DragonBallCharacter';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5304';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export class CharacterService {
  static async getAllCharacters(): Promise<DragonBallCharacter[]> {
    const response = await api.get<DragonBallCharacter[]>('/api/characters');
    return response.data;
  }

  static async getCharacter(id: number): Promise<DragonBallCharacter> {
    const response = await api.get<DragonBallCharacter>(`/api/characters/${id}`);
    return response.data;
  }

  static async createCharacter(character: CreateCharacterRequest): Promise<DragonBallCharacter> {
    const response = await api.post<DragonBallCharacter>('/api/characters', character);
    return response.data;
  }

  static async updateCharacter(id: number, character: UpdateCharacterRequest): Promise<DragonBallCharacter> {
    const response = await api.put<DragonBallCharacter>(`/api/characters/${id}`, character);
    return response.data;
  }

  static async deleteCharacter(id: number): Promise<void> {
    await api.delete(`/api/characters/${id}`);
  }
}