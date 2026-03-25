import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import interviewService from '../services/interviewService';
import Spinner from '../components/UI/Spinner';
import {
  Send,
  StopCircle,
  Clock,
  BrainCircuit,
  User,
  Bot,
} from 'lucide-react';

export default function InterviewChat() {
  const { sessionId } = useParams();
  const navigate = useNavigate();
  const messagesEndRef = useRef(null);
  const inputRef = useRef(null);

  const [messages, setMessages] = useState([]);
  const [input, setInput] = useState('');
  const [sending, setSending] = useState(false);
  const [ending, setEnding] = useState(false);
  const [sessionInfo, setSessionInfo] = useState(null);
  const [loading, setLoading] = useState(true);
  const [elapsed, setElapsed] = useState(0);

  // Cargar sesion existente
  useEffect(() => {
    const loadSession = async () => {
      try {
        const result = await interviewService.getSession(sessionId);
        if (result.success) {
          setSessionInfo(result.data);
          setMessages(result.data.mensajes || []);

          if (result.data.estado !== 'EnCurso') {
            navigate(`/interview/${sessionId}/results`, { replace: true });
            return;
          }
        } else {
          toast.error('No se pudo cargar la entrevista');
          navigate('/interview', { replace: true });
        }
      } catch {
        toast.error('Error al cargar la entrevista');
        navigate('/interview', { replace: true });
      } finally {
        setLoading(false);
      }
    };

    loadSession();
  }, [sessionId, navigate]);

  // Temporizador
  useEffect(() => {
    if (!sessionInfo || sessionInfo.estado !== 'EnCurso') return;

    const startTime = new Date(sessionInfo.fechaInicio).getTime();
    const interval = setInterval(() => {
      setElapsed(Math.floor((Date.now() - startTime) / 1000));
    }, 1000);

    return () => clearInterval(interval);
  }, [sessionInfo]);

  // Auto-scroll
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages, sending]);

  const formatTime = (seconds) => {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
  };

  const handleSend = async () => {
    const texto = input.trim();
    if (!texto || sending) return;

    // Agregar mensaje del candidato inmediatamente
    const msgCandidato = { rol: 'candidato', contenido: texto, timestamp: new Date().toISOString() };
    setMessages((prev) => [...prev, msgCandidato]);
    setInput('');
    setSending(true);

    try {
      const result = await interviewService.sendMessage(sessionId, texto);
      if (result.success) {
        setMessages((prev) => [...prev, result.data]);
      } else {
        toast.error(result.message || 'Error al enviar mensaje');
      }
    } catch {
      toast.error('Error de conexion');
    } finally {
      setSending(false);
      inputRef.current?.focus();
    }
  };

  const handleEnd = async () => {
    if (ending) return;
    setEnding(true);

    try {
      const result = await interviewService.endInterview(sessionId);
      if (result.success) {
        toast.success('Entrevista finalizada');
        navigate(`/interview/${sessionId}/results`, { state: { evaluation: result.data } });
      } else {
        toast.error(result.message || 'Error al finalizar');
        setEnding(false);
      }
    } catch {
      toast.error('Error al finalizar la entrevista');
      setEnding(false);
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center" style={{ minHeight: 'calc(100vh - 60px)' }}>
        <Spinner size="lg" />
      </div>
    );
  }

  const durationLimit = (sessionInfo?.duracionMinutos || 30) * 60;
  const timeProgress = Math.min((elapsed / durationLimit) * 100, 100);
  const timeWarning = timeProgress > 80;

  return (
    <div className="flex flex-col" style={{ height: 'calc(100vh - 56px)' }}>
      {/* Header */}
      <div className="shrink-0 flex items-center justify-between px-5 py-3 border-b border-neutral-200 bg-white">
        <div className="flex items-center gap-3">
          <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-primary-100">
            <BrainCircuit size={18} className="text-primary-600" />
          </div>
          <div>
            <p className="text-sm font-semibold text-neutral-900">
              Entrevista {sessionInfo?.tipoEntrevista}
              {sessionInfo?.tecnologia && ` - ${sessionInfo.tecnologia}`}
            </p>
            <p className="text-[11px] text-neutral-500">
              Nivel {sessionInfo?.nivel}
            </p>
          </div>
        </div>

        <div className="flex items-center gap-4">
          {/* Timer */}
          <div className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm font-mono font-medium ${
            timeWarning ? 'bg-red-50 text-red-600' : 'bg-neutral-100 text-neutral-600'
          }`}>
            <Clock size={14} />
            {formatTime(elapsed)}
            <span className="text-neutral-400 text-xs">/ {sessionInfo?.duracionMinutos}m</span>
          </div>

          {/* End button */}
          <button
            onClick={handleEnd}
            disabled={ending}
            className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm font-medium bg-red-50 text-red-600 hover:bg-red-100 transition-colors disabled:opacity-50"
          >
            {ending ? <Spinner size="sm" /> : <StopCircle size={14} />}
            Finalizar
          </button>
        </div>
      </div>

      {/* Timer progress bar */}
      <div className="shrink-0 h-0.5 bg-neutral-100">
        <div
          className={`h-full transition-all duration-1000 ${timeWarning ? 'bg-red-400' : 'bg-primary-400'}`}
          style={{ width: `${timeProgress}%` }}
        />
      </div>

      {/* Messages */}
      <div className="flex-1 overflow-y-auto px-5 py-4 space-y-4">
        {messages.map((msg, idx) => (
          <div
            key={idx}
            className={`flex gap-3 ${msg.rol === 'candidato' ? 'flex-row-reverse' : ''}`}
          >
            {/* Avatar */}
            <div className={`shrink-0 flex items-center justify-center w-8 h-8 rounded-full ${
              msg.rol === 'candidato'
                ? 'bg-accent-100'
                : 'bg-primary-100'
            }`}>
              {msg.rol === 'candidato'
                ? <User size={14} className="text-accent-600" />
                : <Bot size={14} className="text-primary-600" />
              }
            </div>

            {/* Bubble */}
            <div className={`max-w-[75%] rounded-xl px-4 py-3 ${
              msg.rol === 'candidato'
                ? 'bg-accent-50 border border-accent-200'
                : 'bg-white border border-neutral-200 shadow-sm'
            }`}>
              <p className="text-sm text-neutral-800 whitespace-pre-wrap">{msg.contenido}</p>
              <p className="text-[10px] text-neutral-400 mt-1.5">
                {msg.timestamp && new Date(msg.timestamp).toLocaleTimeString('es', { hour: '2-digit', minute: '2-digit' })}
              </p>
            </div>
          </div>
        ))}

        {/* Typing indicator */}
        {sending && (
          <div className="flex gap-3">
            <div className="shrink-0 flex items-center justify-center w-8 h-8 rounded-full bg-primary-100">
              <Bot size={14} className="text-primary-600" />
            </div>
            <div className="bg-white border border-neutral-200 rounded-xl px-4 py-3 shadow-sm">
              <div className="flex items-center gap-1.5">
                <span className="text-sm text-neutral-500">El entrevistador esta escribiendo</span>
                <span className="flex gap-0.5">
                  <span className="w-1.5 h-1.5 rounded-full bg-neutral-400 animate-bounce" style={{ animationDelay: '0ms' }} />
                  <span className="w-1.5 h-1.5 rounded-full bg-neutral-400 animate-bounce" style={{ animationDelay: '150ms' }} />
                  <span className="w-1.5 h-1.5 rounded-full bg-neutral-400 animate-bounce" style={{ animationDelay: '300ms' }} />
                </span>
              </div>
            </div>
          </div>
        )}

        <div ref={messagesEndRef} />
      </div>

      {/* Input */}
      <div className="shrink-0 border-t border-neutral-200 bg-white px-5 py-3">
        <div className="flex gap-3">
          <textarea
            ref={inputRef}
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Escribe tu respuesta..."
            disabled={sending || ending}
            rows={1}
            className="flex-1 resize-none rounded-lg border border-neutral-300 px-4 py-2.5 text-sm text-neutral-800 placeholder-neutral-400 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent disabled:opacity-50 disabled:bg-neutral-50"
            style={{ minHeight: 42, maxHeight: 120 }}
            onInput={(e) => {
              e.target.style.height = '42px';
              e.target.style.height = `${Math.min(e.target.scrollHeight, 120)}px`;
            }}
          />
          <button
            onClick={handleSend}
            disabled={!input.trim() || sending || ending}
            className="shrink-0 flex items-center justify-center w-10 h-10 rounded-lg bg-primary-600 text-white hover:bg-primary-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Send size={16} />
          </button>
        </div>
      </div>
    </div>
  );
}
