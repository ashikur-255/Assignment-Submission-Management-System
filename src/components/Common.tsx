import { useEffect, useState } from "react";
import { AlertCircle, CheckCircle2, Loader2, Search, X } from "lucide-react";

export function Loading({ text = "Loading..." }: { text?: string }) {
  return <div className="loading"><Loader2 className="spin" size={20}/><span>{text}</span></div>;
}

export function Empty({ title = "Nothing here yet", text = "No records were found." }: { title?: string; text?: string }) {
  return <div className="empty"><div className="empty-icon">◎</div><h3>{title}</h3><p>{text}</p></div>;
}

export function SearchBox({ value, onChange, placeholder = "Search..." }: { value: string; onChange: (v: string) => void; placeholder?: string }) {
  return <div className="search-box"><Search size={18}/><input value={value} onChange={e => onChange(e.target.value)} placeholder={placeholder}/>{value && <button className="icon-btn" onClick={() => onChange("")}><X size={15}/></button>}</div>;
}

export function Modal({ open, title, onClose, children, wide = false }: { open: boolean; title: string; onClose: () => void; children: React.ReactNode; wide?: boolean }) {
  if (!open) return null;
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}>
    <div className={`modal ${wide ? "modal-wide" : ""}`}>
      <div className="modal-head"><h2>{title}</h2><button className="icon-btn" onClick={onClose}><X/></button></div>
      <div className="modal-body">{children}</div>
    </div>
  </div>;
}

export function Toast({ type, message, onClose }: { type: "success"|"error"|"info"; message: string; onClose: () => void }) {
  useEffect(() => { const t = setTimeout(onClose, 4000); return () => clearTimeout(t); }, [onClose]);
  return <div className={`toast ${type}`}><span>{type === "success" ? <CheckCircle2/> : <AlertCircle/>}</span><span>{message}</span><button className="icon-btn" onClick={onClose}><X size={16}/></button></div>;
}

export function Pagination({ page, pageSize, total, onPage }: { page: number; pageSize: number; total: number; onPage: (p: number) => void }) {
  const pages = Math.max(1, Math.ceil(total / pageSize));
  if (pages <= 1) return null;
  return <div className="pagination"><span>{Math.min((page-1)*pageSize+1,total)}–{Math.min(page*pageSize,total)} of {total}</span><div><button disabled={page===1} onClick={() => onPage(page-1)}>Previous</button><b>{page} / {pages}</b><button disabled={page===pages} onClick={() => onPage(page+1)}>Next</button></div></div>;
}

export function StatusBadge({ value }: { value?: string }) {
  const cls = value?.toLowerCase().replace(/\s+/g, "-") ?? "default";
  return <span className={`badge ${cls}`}>{value ?? "—"}</span>;
}

export function useDebounced<T>(value: T, ms = 350) {
  const [state, setState] = useState(value);
  useEffect(() => { const t = setTimeout(() => setState(value), ms); return () => clearTimeout(t); }, [value, ms]);
  return state;
}
