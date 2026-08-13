import { FormEvent, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { ArrowRight, BookOpen, ShieldCheck } from "lucide-react";
import { useAppDispatch, useAppSelector } from "../../hooks";
import { login } from "../../features/auth/authSlice";
import { PasswordInput, Field, TextInput } from "../../components/Forms";

export default function Login() {
  const dispatch = useAppDispatch(); const navigate = useNavigate(); const location = useLocation();
  const { loading, error } = useAppSelector(s => s.auth);
  const [email, setEmail] = useState(""); const [password, setPassword] = useState(""); const [errors, setErrors] = useState("");
  const submit = async (e: FormEvent) => {
    e.preventDefault(); setErrors("");
    if (!/^\S+@\S+\.\S+$/.test(email)) return setErrors("Enter a valid email address.");
    if (password.length < 8) return setErrors("Password must be at least 8 characters.");
    const result = await dispatch(login({ email, password }));
    if (login.fulfilled.match(result)) {
      const role = result.payload.user.role.toLowerCase();
      navigate((location.state as {from?: string} | null)?.from ?? `/${role}`);
    }
  };
  return <div className="auth-page">
    <div className="auth-visual"><div className="auth-brand"><div className="brand-mark">EA</div><strong>EduAssign</strong></div><div className="auth-copy"><span>ACADEMIC MANAGEMENT</span><h1>Everything for assignments, in one focused workspace.</h1><p>Create, submit, review and grade academic work without the usual administrative clutter.</p><div className="feature-pills"><span><ShieldCheck/>Role-based access</span><span><BookOpen/>Simple workflow</span></div></div><small>Built for schools & colleges</small></div>
    <div className="auth-card-wrap"><form className="auth-card" onSubmit={submit}><div className="auth-mobile-logo"><div className="brand-mark">EA</div><b>EduAssign</b></div><div><div className="eyebrow">WELCOME BACK</div><h2>Sign in</h2><p>Use your school account to continue.</p></div>
      {(error || errors) && <div className="alert error">{error || errors}</div>}
      <Field label="Email"><TextInput value={email} onChange={e=>setEmail(e.target.value)} placeholder="you@example.com" autoComplete="email"/></Field>
      <Field label="Password"><PasswordInput value={password} onChange={e=>setPassword(e.target.value)} placeholder="Your password" autoComplete="current-password"/></Field>
      <button className="btn btn-primary btn-block" disabled={loading}>{loading ? "Signing in..." : "Sign in"}<ArrowRight size={18}/></button>
      <p className="auth-footer">Need an account? <Link to="/register">Register as a student</Link></p>
    </form></div>
  </div>;
}
