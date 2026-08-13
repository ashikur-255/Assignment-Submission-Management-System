import { FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { ArrowRight } from "lucide-react";
import { useAppDispatch, useAppSelector } from "../../hooks";
import { register } from "../../features/auth/authSlice";
import { Field, PasswordInput, TextInput } from "../../components/Forms";

export default function Register() {
  const dispatch=useAppDispatch(); const navigate=useNavigate(); const {loading,error}=useAppSelector(s=>s.auth);
  const [form,setForm]=useState({firstName:"",lastName:"",email:"",phone:"",password:"",confirm:""}); const [local,setLocal]=useState("");
  const update=(k:string,v:string)=>setForm(f=>({...f,[k]:v}));
  const submit=async(e:FormEvent)=>{e.preventDefault();setLocal("");if(!form.firstName||!form.lastName)return setLocal("First and last name are required.");if(!/^\S+@\S+\.\S+$/.test(form.email))return setLocal("Enter a valid email.");if(form.password.length<8)return setLocal("Password must be at least 8 characters.");if(form.password!==form.confirm)return setLocal("Passwords do not match.");
  const r = await dispatch(
  register({
    firstName: form.firstName,
    lastName: form.lastName,
    email: form.email,
    phone: form.phone,
    password: form.password,
  })
);
  if(register.fulfilled.match(r))navigate("/login");};
  return <div className="auth-page single"><form className="auth-card register-card" onSubmit={submit}><div className="auth-mobile-logo"><div className="brand-mark">EA</div><b>EduAssign</b></div><div><div className="eyebrow">STUDENT REGISTRATION</div><h2>Create your account</h2><p>Your account will be created with the Student role.</p></div>{(error||local)&&<div className="alert error">{error||local}</div>}
    <div className="form-grid two"><Field label="First name"><TextInput value={form.firstName} onChange={e=>update("firstName",e.target.value)}/></Field><Field label="Last name"><TextInput value={form.lastName} onChange={e=>update("lastName",e.target.value)}/></Field></div>
    <Field label="Email"><TextInput type="email" value={form.email} onChange={e=>update("email",e.target.value)}/></Field>
    <Field label="Phone"><TextInput value={form.phone} onChange={e=>update("phone",e.target.value)}/></Field>
    <div className="form-grid two"><Field label="Password"><PasswordInput value={form.password} onChange={e=>update("password",e.target.value)}/></Field><Field label="Confirm password"><PasswordInput value={form.confirm} onChange={e=>update("confirm",e.target.value)}/></Field></div>
    <button className="btn btn-primary btn-block" disabled={loading}>{loading?"Creating...":"Create account"}<ArrowRight/></button><p className="auth-footer">Already registered? <Link to="/login">Sign in</Link></p>
  </form></div>;
}
