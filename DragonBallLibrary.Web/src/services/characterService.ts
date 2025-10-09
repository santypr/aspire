import axios from 'axios';
import { DragonBallCharacter, CreateCharacterRequest, UpdateCharacterRequest } from '../types/DragonBallCharacter';

// Extend the Window interface to include __env
declare global {
  interface Window {
    __env?: {
      REACT_APP_API_URL?: string;
      REACT_APP_ENVIRONMENT?: string;
    };
  }
}

// Helper function to get API URL from runtime or build-time environment variables
const getApiUrl = (): string => {
  // Runtime environment (from window.__env)
  if (typeof window !== 'undefined' && window.__env && window.__env.REACT_APP_API_URL) {
    return window.__env.REACT_APP_API_URL;
  }
  
  // Build-time environment (webpack DefinePlugin)
  if (typeof process !== 'undefined' && process.env && process.env.REACT_APP_API_URL) {
    return process.env.REACT_APP_API_URL;
  }
  
  // Default fallback
  return 'http://localhost:5304';
};

const API_BASE_URL = getApiUrl();

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