import { Plus, RefreshCw } from "lucide-react";

export default function Page({ title, subtitle, action, onRefresh, children }: { title: string; subtitle?: string; action?: React.ReactNode; onRefresh?: () => void; children: React.ReactNode }) {
  return <div className="page">
    <div className="page-heading"><div><div className="eyebrow">EDUASSIGN</div><h1>{title}</h1>{subtitle && <p>{subtitle}</p>}</div><div className="heading-actions">{onRefresh && <button className="btn btn-secondary" onClick={onRefresh}><RefreshCw size={17}/>Refresh</button>}{action ?? null}</div></div>
    {children}
  </div>;
}
export const AddButton = ({ onClick, children = "Add new" }: { onClick: () => void; children?: React.ReactNode }) => <button className="btn btn-primary" onClick={onClick}><Plus size={17}/>{children}</button>;
